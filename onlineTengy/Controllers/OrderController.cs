using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onlineTengy.Data;
using onlineTengy.Models;
using onlineTengy.Models.OrderDetailsViewModels;
using onlineTengy.Utility;

namespace onlineTengy.Controllers
{
    public class OrderController : Controller
    {

        private readonly ApplicationDbContext _context;

        private  int  pageSize=2;
        public OrderController(ApplicationDbContext d)
        {
            _context = d;

        }

        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var ClaimIdentity = (ClaimsIdentity)this.User.Identity;

            var cId = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //OrderHeader order = _context.OrderHeaders.Where(c => c.Id == id && c.UserId == cId.Value).FirstOrDefault();


            OrderDetailViewModel orderDetailViewModel = new OrderDetailViewModel
            {
                OrderHeader = _context.OrderHeaders.Where(c => c.Id == id && c.UserId == cId.Value).FirstOrDefault(),
                OrderDetails = await _context.OrderDetails.Where(c => c.OrderId == id).ToListAsync()
            };

            return View(orderDetailViewModel);
        }

        [Authorize]
        public async Task<IActionResult> OrderHistory(int page=1)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;

            var claimId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<OrderDetailViewModel> orderDetailViewsVM = new List<OrderDetailViewModel>();

            //OrderListViewModel orderListVCM = new OrderListViewModel()
            //{

            //    orders =new List<OrderDetailViewModel>()
            //};

            List<OrderHeader> orderHeadersList =await _context.OrderHeaders.Where(c => c.UserId.Equals(claimId.Value)).OrderByDescending(c => c.OrderDate).ToListAsync();
            
            var pageSie = 5;
            foreach (var item in orderHeadersList)
            {
                OrderDetailViewModel order = new OrderDetailViewModel
                {
                    OrderHeader = item,
                    OrderDetails = _context.OrderDetails.Where(c => c.OrderId == item.Id).ToList()

                };

                orderDetailViewsVM.Add(order);
            }

            ////Coutn the total Number of the Order 
            //var count = orderListVCM.orders.Count;
            //orderListVCM.orders = orderListVCM.orders.OrderBy(c => c.OrderHeader.Id)
            //    .Skip((productPage - 1) * pageSize)
            //    .Take(pageSize).ToList();


            //orderListVCM.PageInfo = new PageInfo
            //{
            //    CurrentPage = productPage,
            //    ItemPerPage = pageSize,
            //    TotalItem = count
            //};
            
            return View(orderDetailViewsVM);

            //return View(orderListVCM);
        }

        [Authorize(Roles = SD.AdminEndUser)]
        public async Task<IActionResult> ManageOrder()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claimId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //Get the List of orderHeader
            List<OrderHeader> orderHeadersList = _context.OrderHeaders.Where(c => c.Status == SD.StatusSubmitted || c.Status == SD.StatusInProcess)
                .OrderByDescending(c => c.PickUpDate).ToList();

            List<OrderDetailViewModel> orderDetailViewModelsVM = new List<OrderDetailViewModel>();


            //Iterate the list of orderheader 
            foreach (var item in orderHeadersList)
            {
                OrderDetailViewModel orderDetailView = new OrderDetailViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _context.OrderDetails.Where(c => c.OrderId == item.Id).ToListAsync()

                };

                orderDetailViewModelsVM.Add(orderDetailView);
            }

            return View(orderDetailViewModelsVM);
        }

        [Authorize(Roles = SD.AdminEndUser)]
        public async Task<IActionResult> OrderPrepared(int id)
        {
            OrderHeader orderHeader = _context.OrderHeaders.Find(id);
            orderHeader.Status = SD.StatusInProcess;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageOrder), "Order");
        }

        [Authorize(Roles = SD.AdminEndUser)]
        public async Task<IActionResult> OrderReady(int id)
        {
            OrderHeader orderHeader = _context.OrderHeaders.Find(id);
            orderHeader.Status = SD.StatusReady;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageOrder), "Order");
        }


        [Authorize(Roles = SD.AdminEndUser)]
        public async Task<IActionResult> OrderCancel(int id)
        {
            OrderHeader orderHeader = _context.OrderHeaders.Find(id);
            orderHeader.Status = SD.StatusCancelled;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageOrder), "Order");
        }

        [HttpPost]
        public JsonResult OrderSt(int id, string s)
        {
            OrderHeader orderHeader = _context.OrderHeaders.Find(id);
            orderHeader.Status = s.ToString();

            _context.SaveChanges();
            string me = s;
            return this.Json(me);
        }




        [Authorize(Roles = SD.AdminEndUser)]
        public IActionResult OrderPickUp(string searchOrder, string searcPhone, string searchEmail)
        {
            List<OrderDetailViewModel> orderDetailViewModelsVM = new List<OrderDetailViewModel>();

            if (searchOrder != null || searcPhone != null || searchEmail != null)
            {
                var user = new ApplicationUser();

                List<OrderHeader> orderHeaders_List = new List<OrderHeader>();

                if (searchOrder != null)
                {
                    orderHeaders_List = _context.OrderHeaders.Where(x => x.Id == Convert.ToInt32(searchOrder)).ToList();
                }

                else
                {
                    if (searchEmail != null)
                    {
                        user = _context.ApplicationUsers.Where(x => x.Email.ToLower().Contains(searchEmail.ToLower())).FirstOrDefault();
                    }
                    else
                    {
                        if (searcPhone != null)
                        {
                            user = _context.ApplicationUsers.Where(x => x.PhoneNumber.ToLower().Contains(searcPhone.ToLower())).FirstOrDefault();
                        }
                    }

                }
                if (user != null || orderHeaders_List.Count > 0)
                {
                    if (orderHeaders_List.Count == 0)
                    {
                        orderHeaders_List = _context.OrderHeaders.Where(x => x.UserId == user.Id).OrderByDescending(c => c.OrderDate).ToList();
                    }


                    foreach (OrderHeader item in orderHeaders_List)
                    {

                        OrderDetailViewModel orderDetailView = new OrderDetailViewModel
                        {
                            OrderHeader = item,
                            OrderDetails = _context.OrderDetails.Where(c => c.OrderId == item.Id).ToList()

                        };

                        orderDetailViewModelsVM.Add(orderDetailView);
                    }
                }
            }
            else
            {



                //Get the List of orderHeader
                List<OrderHeader> orderHeadersList = _context.OrderHeaders.Where(c => c.Status == SD.StatusReady)
                    .OrderByDescending(c => c.PickUpDate).ToList();




                //Iterate the list of orderheader 
                foreach (var item in orderHeadersList)
                {
                    OrderDetailViewModel orderDetailView = new OrderDetailViewModel
                    {
                        OrderHeader = item,
                        OrderDetails = _context.OrderDetails.Where(c => c.OrderId == item.Id).ToList()

                    };

                    orderDetailViewModelsVM.Add(orderDetailView);
                }
                
            }

            return View(orderDetailViewModelsVM);
        }


        //[Authorize(Roles =SD.AdminEndUser)]
        //public IActionResult OrderPickUpDetails(int id)
        //{
        //    OrderDetailViewModel orderDetailView = new OrderDetailViewModel
        //    {
        //        OrderHeader = _context.OrderHeaders.Where(x => x.Id == id).FirstOrDefault()
        //    };

        //    orderDetailView.OrderHeader.ApplicationUser = _context.Users.Where(c => c.Id == orderDetailView.OrderHeader.UserId).FirstOrDefault();
        //    orderDetailView.OrderDetails = _context.OrderDetails.Where(x => x.OrderId == orderDetailView.OrderHeader.Id).ToList();

        //    return View(orderDetailView);

        //}




        [Authorize(Roles = SD.AdminEndUser)]

        public IActionResult OrderPickUpDetails(int id)
        {
            OrderDetailViewModel order = new OrderDetailViewModel
            {
                OrderHeader = _context.OrderHeaders.Include(x => x.ApplicationUser).Where(x => x.Id == id).FirstOrDefault()

            };
            order.OrderDetails = _context.OrderDetails.Where(c => c.OrderId == order.OrderHeader.Id).ToList();

            return View(order);
        }


        [Authorize(Roles = SD.AdminEndUser)]
        [ActionName("OrderPickUpDetails")]
        [HttpPost]
        public async Task<IActionResult> OrderPickUpDetailsPost(int id)
        {
            OrderHeader orderHeader = _context.OrderHeaders.Find(id);

            orderHeader.Status = SD.StatusCompleted;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageOrder), "Order");

        }


        public IActionResult OrderSummaryReport(int id)
        {
            return View();
        }


        [HttpPost]
        public IActionResult OrderSummaryReport(OrderExportViewModel orderExportVM)
        {
            List<OrderHeader> orderHeadersList = _context.OrderHeaders.Where(c => c.OrderDate >= orderExportVM.startDate && c.OrderDate
              <= orderExportVM.endDate).ToList();

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();

            List<OrderDetails> IndividualorderDetailsList = new List<OrderDetails>();

            foreach (var orderHeader in orderHeadersList)
            {
                IndividualorderDetailsList = _context.OrderDetails.Where(c => c.OrderId == orderHeader.Id).ToList();

                foreach (var individualOrder in IndividualorderDetailsList)
                {
                    orderDetailsList.Add(individualOrder);
                }
            }

            byte[] bytes = Encoding.ASCII.GetBytes(ConvertToString(orderDetailsList));


            return File(bytes,"application/text","Orderdetail.csv");
        }

        public string ConvertToString<T>(IList<T>data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            table.Columns.Remove("OrderHeader");
            table.Columns.Remove("MenuItemId");
            table.Columns.Remove("MenuItem");
            table.Columns.Remove("Description");

            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = table.Columns.Cast<DataColumn>().Select(column => column.ColumnName);

            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow  row in table.Rows)
            {
                IEnumerable<string> field = row.ItemArray.Select(f => f.ToString());

                sb.AppendLine(string.Join(",", field));
            }

            return sb.ToString();
        }
    }
}