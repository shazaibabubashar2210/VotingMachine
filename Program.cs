using System;
using System.Data;

namespace Assignment3
{
    internal class Program
    {

        static int customerCount = 0;
        static void Main(string[] args)
        {
            OrderManager orderManager = new OrderManager();
            DataSet orderDataSet = null;
            int orderID;

            int choice;

            while (true)
            {
                Console.WriteLine("==== QueenShop Order Management System ====");

                Console.WriteLine("Enter 1 for InsertCustomers");
                Console.WriteLine("Enter 2 for InsertProducts");
                Console.WriteLine("Enter 3 for InsertOrders");
                Console.WriteLine("Enter 4 for GetOrdersByOrderID");
                Console.WriteLine("Enter 5 for ModifyOrderDetails");
                Console.WriteLine("Enter 6 for AddNewOrder");
                Console.WriteLine("Enter 7 for RemoveOrder");
                Console.WriteLine("Enter 8 for SynchronizeWithDatabase");
                Console.WriteLine("Enter 9 for DisplayOrders");


                Console.Write("Enter Your choice: ");

                choice = Convert.ToInt32(Console.ReadLine());
                while (choice < 1 || choice > 10)
                {
                    Console.Write("Please choose  your options from (1-9): ");
                    choice = Convert.ToInt32(Console.ReadLine());
                }
                switch (choice)
                {
                    case 1:

                        Console.WriteLine("------------------------------------");
                        Console.WriteLine("1. Add Customer");
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine("Enter Customer Details:");

                        Console.Write("Customer Name: ");
                        string customerName = Console.ReadLine();

                        Console.Write("Customer Address: ");
                        string customerAddress = Console.ReadLine();

                        Console.Write("Customer Contact: ");
                        string customerContact = Console.ReadLine();

                        customerCount++;

                        string customerID = "Cus-" + customerCount.ToString();

                        Console.WriteLine("Generated CustomerID: " + customerID);

                        orderManager.InsertCustomers(customerID, customerName, customerAddress, customerContact);
                        break;

                    case 2:
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine("2. Add Product");
                        Console.WriteLine("------------------------------------");

                        string[] productCodes = new string[5];

                        for (int i = 0; i < 5; i++)
                        {
                            Console.WriteLine($"Enter details for Product {i + 1}:");
                            string productCode;
                            bool isUnique;

                            do
                            {
                                Console.Write("Enter Product Code (1 to 5): ");
                                productCode = Console.ReadLine();
                                isUnique = true;

                                foreach (string code in productCodes)
                                {
                                    if (code == productCode)
                                    {
                                        isUnique = false;
                                        Console.WriteLine("Product Code already exists. Please enter a unique code.");
                                        break;
                                    }
                                }
                            } while (!isUnique);

                            Console.Write("Product Name: ");
                            string productName = Console.ReadLine();

                            Console.Write("Product Price: ");
                            decimal productPrice = decimal.Parse(Console.ReadLine());

                            Console.Write("Product Picture: ");
                            string productPicture = Console.ReadLine();

                            orderManager.InsertProducts(productCode, productName, productPrice, productPicture);

                            Console.WriteLine("Product added successfully.");

                            productCodes[i] = productCode;
                        }

                        break;

                    case 3:
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine("3. Add Order");
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine("Enter Order Details:");

                        Console.Write("Order ID: ");
                        string OrderID = Console.ReadLine();

                        Console.Write("Customer ID: ");
                        string CustomerID = Console.ReadLine();

                        Console.Write("Customer Name: ");
                        string CustomerName = Console.ReadLine();

                        Console.Write("Customer Contact: ");
                        string CustomerContact = Console.ReadLine();

                        Console.Write("Customer Address: ");
                        string CustomerAddress = Console.ReadLine();

                        Console.Write("Product Code: ");
                        string ProductCode = Console.ReadLine();

                        Console.Write("Price: ");
                        decimal Price = decimal.Parse(Console.ReadLine());

                        Console.Write("Product Size: ");
                        string ProductSize = Console.ReadLine();

                        Console.Write("Product Quantity: ");
                        int ProductQuantity = int.Parse(Console.ReadLine());

                        orderManager.InsertOrders(OrderID, CustomerID, CustomerName, CustomerContact, CustomerAddress, ProductCode, Price, ProductSize, ProductQuantity);
                        break;


                    case 4:
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine("4. Get Orders By Order ID");
                        Console.WriteLine("------------------------------------");
                        Console.Write("Enter Order ID: ");
                        orderID = int.Parse(Console.ReadLine());
                        orderDataSet = orderManager.GetOrdersByOrderID(orderID);

                        Console.WriteLine("Orders retrieved successfully.");
                        Console.ReadKey();
                        break;


                    case 5:
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine("5. Modify Order Details");
                        Console.WriteLine("------------------------------------");

                        Console.Write("Enter Order ID to modify details: ");
                        int orderIDToModify = int.Parse(Console.ReadLine());

                        DataSet ordersToModify = orderManager.GetOrdersByOrderID(orderIDToModify);

                        if (ordersToModify.Tables["Orders"].Rows.Count > 0)
                        {
                            Console.WriteLine("Orders before modification:");
                            foreach (DataRow row in ordersToModify.Tables["Orders"].Rows)
                            {
                                Console.WriteLine($"OrderID: {row["OrderID"]}, Quantity: {row["Quantity"]}");
                            }

                            orderManager.ModifyOrderDetails(ordersToModify.Tables["Orders"]);

                            orderManager.SynchronizeWithDatabase(ordersToModify);

                            Console.WriteLine("Order details modified successfully and synchronized with the database.");
                        }
                        else
                        {
                            Console.WriteLine($"No orders found with Order ID: {orderIDToModify}");
                        }
                        Console.ReadKey();
                        break;

                    case 6:
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine("6. Add New Order");
                        Console.WriteLine("------------------------------------");

                        DataTable newOrderTable = new DataTable();
                        newOrderTable.Columns.Add("CustomerName", typeof(string));
                        newOrderTable.Columns.Add("CustomerContact", typeof(string));
                        newOrderTable.Columns.Add("CustomerAddress", typeof(string));
                        newOrderTable.Columns.Add("ProductCode", typeof(string));
                        newOrderTable.Columns.Add("Price", typeof(decimal));
                        newOrderTable.Columns.Add("ProductSize", typeof(string));
                        newOrderTable.Columns.Add("ProductQuantity", typeof(int));

                        orderManager.AddNewOrder(newOrderTable);
                        break;

                    case 7:
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine("7. Remove Order");
                        Console.WriteLine("------------------------------------");

                        Console.Write("Enter Order ID to delete: ");
                        int orderIDToRemove = int.Parse(Console.ReadLine());

                        DataSet ordersToRemove = orderManager.GetOrdersByOrderID(orderIDToRemove);

                        if (ordersToRemove.Tables["Orders"].Rows.Count > 0)
                        {
                            orderManager.RemoveOrder(ordersToRemove.Tables["Orders"]);
                            Console.WriteLine("Order removed successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"No orders found with Order ID: {orderIDToRemove}");
                        }
                        break;

                    case 8:
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine("8. Synchronize With Database");
                        Console.WriteLine("------------------------------------");

                        orderManager.SynchronizeWithDatabase(orderDataSet);
                        Console.WriteLine("Changes synchronized with the database.");
                        break;

                    case 9:
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine("9. Display Orders");
                        Console.WriteLine("------------------------------------");

                        orderManager.DisplayOrders();
                        break;
               }
               
            }
 
        }

    }
}