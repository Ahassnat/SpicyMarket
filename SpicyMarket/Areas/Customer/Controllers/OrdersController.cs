using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpicyMarket.Data;
using SpicyMarket.Models;
using SpicyMarket.Models.ViewModel;
using SpicyMarket.Utility;

namespace SpicyMarket.Areas.Customer.Controllers
{
    [Area(SD.Customer)]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var orderDetalisVM = new OrderDetailsViewModel()
            {
                OrderHeader = await _context.OrderHeaders.Include(x => x.ApplicationUser)
                                                        .FirstOrDefaultAsync(x => x.UserId == claim.Value &&
                                                                                  x.Id ==id),
                OrderDetails =await _context.OrderDetails.Where(m=>m.OrderId == id).ToListAsync()
            };


            return View(orderDetalisVM);
        }

        private int pageSize = 2;

        [Authorize]
        public async Task<IActionResult> OrderHistory(int pageNumber = 1)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //var orderDetailsVMList = new List<OrderDetailsViewModel>();
            var orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>()
            };

            var orderHeadersList = await _context.OrderHeaders.Include(x => x.ApplicationUser)
                                                          .Where(m => m.UserId == claim.Value)
                                                          .ToListAsync();
            foreach (var orderHeader in orderHeadersList)
            {
                var orderDetailsVM = new OrderDetailsViewModel()
                {
                    OrderHeader = orderHeader,
                    OrderDetails = await _context.OrderDetails.Where(x => x.OrderId == orderHeader.Id)
                                                              .ToListAsync()
                };
                orderListVM.Orders.Add(orderDetailsVM);
            }
            // Paging Information 
            var count = orderListVM.Orders.Count; // represent the Total page in PagingInfo Class
            orderListVM.Orders = orderListVM.Orders.OrderByDescending(x => x.OrderHeader.Id)
                                                   .Skip((pageNumber - 1) * pageSize)
                                                   .Take(pageSize)
                                                   .ToList();

            orderListVM.PagingInfo = new PagingInfo()
            {
                CurrentPage = pageNumber,
                RecordsPerPage = pageSize,
                TotalRecords = count,
                UrlParam = "/Customer/Orders/OrderHistory?pageNumber=:"
            };
           
            return View(orderListVM);
        }
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            var orderDetailsVM = new OrderDetailsViewModel()
            {
                OrderHeader = await _context.OrderHeaders.Include(x => x.ApplicationUser)
                                                      .FirstOrDefaultAsync(x => x.Id == id),
                OrderDetails = await _context.OrderDetails.Where(x => x.OrderId == id).ToListAsync()
            };
            return PartialView("_IndividualOrderDetalis", orderDetailsVM);
        }

        public async Task<IActionResult> GetOrderStatus(int id)
        {
            var orderHeader = await _context.OrderHeaders.FindAsync(id);
            return PartialView("_OrderStatus", orderHeader.Status);
        }

        [Authorize(Roles = SD.ManagerUser +","+ SD.KitchenUser)]
        public async Task<IActionResult> ManageOrder()
        {
            var orderDetalisVMList = new List<OrderDetailsViewModel>();
            var orderHeaderList = await _context.OrderHeaders
                                        .Where(x => x.Status == SD.StatusSubmitted || x.Status == SD.StatusInProcess)
                                        .ToListAsync();
            foreach (var orderHeader in orderHeaderList)
            {
                var orderDetailsVM = new OrderDetailsViewModel
                {
                    OrderHeader = orderHeader,
                    OrderDetails = await _context.OrderDetails.Where(x => x.OrderId == orderHeader.Id).ToListAsync()
                };
                orderDetalisVMList.Add(orderDetailsVM);
            }
            return View(orderDetalisVMList.OrderBy(x => x.OrderHeader.PickUpTime).ToList());
        }


        [Authorize(Roles = SD.ManagerUser + "," + SD.KitchenUser)]
        public async Task<IActionResult> OrderPrepare( int orderId)
        {
            var orderHeader = await _context.OrderHeaders.FindAsync(orderId);
            orderHeader.Status = SD.StatusInProcess;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageOrder));
        }

        [Authorize(Roles = SD.ManagerUser + "," + SD.KitchenUser)]
        public async Task<IActionResult> OrderReady(int orderId)
        {
            var orderHeader = await _context.OrderHeaders.FindAsync(orderId);
            orderHeader.Status = SD.StatusReady;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageOrder));
        }

        [Authorize(Roles = SD.ManagerUser + "," + SD.KitchenUser)]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var orderHeader = await _context.OrderHeaders.FindAsync(orderId);
            orderHeader.Status = SD.StatusCancelled;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageOrder));
        }

        // Order Pickup
        [Authorize(Roles = SD.ManagerUser + "," + SD.FrontDeskUser)]
        public async Task<IActionResult> OrderPickup(int pageNumber = 1, string searchName=null,string searchPhone=null, string searchEmail=null)
        {
            var orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>()
            };

            var param = new StringBuilder();
            param.Append("/Customer/Orders/OrderPickup?pageNumber=:");
            param.Append("&searchName=");
            if(searchName != null)
            {
                param.Append(searchName); 
            }
            else
            {
                searchName = "";
            }

            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }
            else
            {
                searchPhone = "";
            }

            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }
            else
            {
                searchEmail = "";
            }
            var orderHeadersList = await _context.OrderHeaders.OrderByDescending(x=>x.OrderDate)
                                                              .Include(x => x.ApplicationUser)
                                                          .Where(x=>x.Status== SD.StatusReady && 
                                                                 x.PickUpName.Contains(searchName) &&
                                                                 x.PhoneNumber.Contains(searchPhone) &&
                                                                 x.ApplicationUser.Email.Contains(searchEmail))
                                                          .ToListAsync();
            foreach (var orderHeader in orderHeadersList)
            {
                var orderDetailsVM = new OrderDetailsViewModel()
                {
                    OrderHeader = orderHeader,
                    OrderDetails = await _context.OrderDetails.Where(x => x.OrderId == orderHeader.Id)
                                                              .ToListAsync()
                };
                orderListVM.Orders.Add(orderDetailsVM);
            }
            // Paging Information 
            var count = orderListVM.Orders.Count; // represent the Total page in PagingInfo Class
            orderListVM.Orders = orderListVM.Orders.OrderByDescending(x => x.OrderHeader.Id)
                                                   .Skip((pageNumber - 1) * pageSize)
                                                   .Take(pageSize)
                                                   .ToList();

            orderListVM.PagingInfo = new PagingInfo()
            {
                CurrentPage = pageNumber,
                RecordsPerPage = pageSize,
                TotalRecords = count,
                UrlParam = param.ToString()
            };

            return View(orderListVM);
        }

    }
}
