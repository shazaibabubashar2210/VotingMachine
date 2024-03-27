using System;
using System.Data.SqlClient;
using System.Data;


namespace Assignment3
{
    internal class OrderManager
    {
        string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=OrderManager;Integrated Security=True;Connect Timeout=30;Encrypt=False;";


        public void InsertCustomers(string customerID, string customerName, string customerAddress, string customerContact)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string insertionQuery = @"INSERT INTO Customers (CustomerID, CustomerName, CustomerAddress, CustomerContact)
                      VALUES
                        (@CustomerID, @CustomerName, @CustomerAddress, @CustomerContact)";

                    using (SqlCommand command = new SqlCommand(insertionQuery, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerID", customerID);
                        command.Parameters.AddWithValue("@CustomerName", customerName);
                        command.Parameters.AddWithValue("@CustomerAddress", customerAddress);
                        command.Parameters.AddWithValue("@CustomerContact", customerContact);

                        command.ExecuteNonQuery();
                        Console.WriteLine("Customer data inserted successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting customer data: {ex.Message}");
            }
        }


        public void InsertProducts(string productCode, string productName, decimal productPrice, string productPicture)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string insertionQuery = @"INSERT INTO Products (ProductCode, ProductName, ProductPrice, ProductPicture)
                          VALUES
                            (@ProductCode, @ProductName, @ProductPrice, @ProductPicture)";

                    using (SqlCommand command = new SqlCommand(insertionQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ProductCode", productCode);
                        command.Parameters.AddWithValue("@ProductName", productName);
                        command.Parameters.AddWithValue("@ProductPrice", productPrice);
                        command.Parameters.AddWithValue("@ProductPicture", productPicture);

                        command.ExecuteNonQuery();
                        Console.WriteLine("Product data inserted successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting product data: {ex.Message}");
            }
        }


        public void InsertOrders(string orderID, string customerID, string customerName, string customerContact, string customerAddress, string productCode, decimal price, string productSize, int productQuantity)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string insertionQuery = @"INSERT INTO Orders (OrderID, CustomerID, CustomerName, CustomerContact, CustomerAddress, ProductCode, Price, ProductSize, ProductQuantity)
                          VALUES
                            (@OrderID, @CustomerID, @CustomerName, @CustomerContact, @CustomerAddress, @ProductCode, @Price, @ProductSize, @ProductQuantity)";

                    using (SqlCommand command = new SqlCommand(insertionQuery, connection))
                    {
                        command.Parameters.AddWithValue("@OrderID", orderID);
                        command.Parameters.AddWithValue("@CustomerID", customerID);
                        command.Parameters.AddWithValue("@CustomerName", customerName);
                        command.Parameters.AddWithValue("@CustomerContact", customerContact);
                        command.Parameters.AddWithValue("@CustomerAddress", customerAddress);
                        command.Parameters.AddWithValue("@ProductCode", productCode);
                        command.Parameters.AddWithValue("@Price", price);
                        command.Parameters.AddWithValue("@ProductSize", productSize);
                        command.Parameters.AddWithValue("@ProductQuantity", productQuantity);

                        command.ExecuteNonQuery();
                        Console.WriteLine("Order data inserted successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting order data: {ex.Message}");
            }
        }



        public DataSet GetOrdersByOrderID(int orderID)
        {
            DataSet orderDataSet = new DataSet();

            try
            {
                string query = "SELECT * FROM Orders WHERE OrderID = @OrderID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@OrderID", orderID);
                    dataAdapter.Fill(orderDataSet, "Orders");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving orders: {ex.Message}");
            }

            return orderDataSet;
        }



        public void ModifyOrderDetails(DataTable ordersTable)
        {
            Console.WriteLine("Modify order details:");

            foreach (DataRow row in ordersTable.Rows)
            {
                Console.Write($"Enter new quantity for order {row["OrderID"]}: ");

                try
                {
                    if (int.TryParse(Console.ReadLine(), out int newQuantity))
                    {
                        row["Quantity"] = newQuantity;
                        Console.WriteLine($"Order {row["OrderID"]} quantity updated successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a valid integer value for quantity.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }



        public void AddNewOrder(DataTable ordersTable)
        {
            try
            {
                Console.WriteLine("Add new order:");
                DataRow newRow = ordersTable.NewRow();

                Console.Write("Enter Customer Name: ");
                newRow["CustomerName"] = Console.ReadLine();

                Console.Write("Enter Customer Contact: ");
                newRow["CustomerContact"] = Console.ReadLine();

                Console.Write("Enter Customer Address: ");
                newRow["CustomerAddress"] = Console.ReadLine();

                Console.Write("Enter Product Code: ");
                newRow["ProductCode"] = Console.ReadLine();

                Console.Write("Enter Price: ");
                newRow["Price"] = decimal.Parse(Console.ReadLine());

                Console.Write("Enter Product Size: ");
                newRow["ProductSize"] = Console.ReadLine();

                Console.Write("Enter Product Quantity: ");
                newRow["ProductQuantity"] = int.Parse(Console.ReadLine());

                ordersTable.Rows.Add(newRow);

                Console.WriteLine("New order added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding new order: {ex.Message}");
            }
        }

        public void RemoveOrder(DataTable ordersTable)
        {
            try
            {
                Console.WriteLine("Remove order:");
                Console.Write("Enter Order ID to delete: ");
                if (int.TryParse(Console.ReadLine(), out int orderID))
                {
                    DataRow[] foundRows = ordersTable.Select($"OrderID = {orderID}");
                    if (foundRows.Length > 0)
                    {
                        foundRows[0].Delete();
                        Console.WriteLine("Order removed successfully from DataTable.");
                    }
                    else
                    {
                        Console.WriteLine("Order not found.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid Order ID.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing order: {ex.Message}");
            }
        }

        public void SynchronizeWithDatabase(DataSet orderDataSet)
        {
            try
            {
                Console.WriteLine("Current Orders:");
                foreach (DataRow row in orderDataSet.Tables["Orders"].Rows)
                {
                    Console.WriteLine($"OrderID: {row["OrderID"]}, Quantity: {row["Quantity"]}");
                }


                Console.WriteLine("Modify order details:");

                foreach (DataRow row in orderDataSet.Tables["Orders"].Rows)
                {
                    Console.Write($"Enter new quantity for order {row["OrderID"]}: ");

                    try
                    {

                        if (int.TryParse(Console.ReadLine(), out int newQuantity))
                        {
                            row["Quantity"] = newQuantity;
                            Console.WriteLine($"Order {row["OrderID"]} quantity updated successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please enter a valid integer value for quantity.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM Orders", connection);
                    SqlCommandBuilder builder = new SqlCommandBuilder(dataAdapter);

                    dataAdapter.Update(orderDataSet, "Orders");
                    Console.WriteLine("Changes synchronized with the database.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error synchronizing with the database: {ex.Message}");
            }
        }


        public void DisplayOrders()
        {
            try
            {
                Console.WriteLine("Displaying Orders:");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Orders";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Console.WriteLine($"OrderID: {reader["OrderID"]}, CustomerID: {reader["CustomerID"]}, CustomerName: {reader["CustomerName"]}, ProductCode: {reader["ProductCode"]}, Price: {reader["Price"]}, ProductQuantity: {reader["ProductQuantity"]}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error displaying orders: {ex.Message}");
            }
        }
    }
}