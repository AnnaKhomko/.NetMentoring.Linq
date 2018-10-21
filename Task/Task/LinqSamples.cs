// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using SampleSupport;
using Task.Data;

// Version Mad01

namespace SampleQueries
{
    [Title("LINQ Module")]
    [Prefix("Linq")]
    public class LinqSamples : SampleHarness
    {

        private DataSource dataSource = new DataSource();

        [Category("Restriction Operators")]
        [Title("Where - Task 1")]
        [Description("This sample uses the where clause to find all elements of an array with a value less than 5.")]
        public void Linq1()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var lowNums =
                from num in numbers
                where num < 5
                select num;

            Console.WriteLine("Numbers < 5:");
            foreach (var x in lowNums)
            {
                Console.WriteLine(x);
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 2")]
        [Description("This sample return return all presented in market products")]

        public void Linq2()
        {
            var products =
                from p in dataSource.Products
                where p.UnitsInStock > 0
                select p;

            foreach (var p in products)
            {
                ObjectDumper.Write(p);
            }
        }

        [Category("Restriction Operators")]
        [Title("Task 001")]
        [Description("This sample returns list of customers with the total orders sum greater than x")]
        public void Linq001()
        {
            int x = 5000;

            var customers = dataSource.Customers
                .Where(c => c.Orders.Select(o => o.Total).Sum() > x)
                .Select(c => new
                {
                    customerId = c.CustomerID,
                    totalOrderSum = c.Orders.Select(o => o.Total).Sum()
                });

            ObjectDumper.Write($"list of customers with the total orders sum greater than {x}");
            foreach (var customer in customers)
            {
                ObjectDumper.Write($"Customer: {customer.customerId}, total order sum: {customer.totalOrderSum}");
            }

            x = 100000;
            ObjectDumper.Write($"list of customers with the total orders sum greater than {x}");
            foreach (var customer in customers)
            {
                ObjectDumper.Write($"Customer: {customer.customerId}, total order sum: {customer.totalOrderSum}");
            }
        }

        [Category("Grouping Operators")]
        [Title("Task 002")]
        [Description("This sample return list of customers and suppliers from the same country and city")]
        public void Linq002()
        {
            var customersWithSuppliers = dataSource.Customers
                 .Select(c => new
                 {
                     customer = c,
                     supplierNames = dataSource.Suppliers.Where(s => s.City == c.City && s.Country == c.Country)
                     .Select(s => s.SupplierName)
                 });

            ObjectDumper.Write(" list of customers and suppliers from the same country and city without grouping");
            foreach (var customer in customersWithSuppliers)
            {
                ObjectDumper.Write($"CustomerId: {customer.customer.CustomerID} " +
                    $"List of suppliers: {string.Join(", ", customer.supplierNames)}");
            }

            var result = dataSource.Customers.GroupJoin(dataSource.Suppliers,
                c => new { c.City, c.Country },
                s => new { s.City, s.Country },
                (c, s) => new { Customer = c, Suppliers = s.Select(x => x.SupplierName) });

            ObjectDumper.Write(" list of customers and suppliers from the same country and city with  grouping");
            foreach (var c in result)
            {
                ObjectDumper.Write($"CustomerId: {c.Customer.CustomerID} " +
                    $"List of suppliers: {string.Join(", ", c.Suppliers)}");
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 3")]
        [Description("This sample return list of customers where on order sum is greater than x")]
        public void Linq003()
        {
            decimal x = 15000;

            var customers = dataSource.Customers.Where(c => c.Orders.Any(o => o.Total > x));

            ObjectDumper.Write($"list of customers where on order sum is greater than {x}");
            foreach (var customer in customers)
            {
                ObjectDumper.Write($"Customer: {customer.CustomerID}, list of orders:");
                foreach (var order in customer.Orders)
                {
                    ObjectDumper.Write($"Order sum:{order.Total}");
                }
            }
        }

        [Category("Restriction Operators")]
        [Title("Task 004")]
        [Description("This sample returns list of customers with date of first order")]
        public void Linq004()
        {
            var customers = dataSource.Customers
                .Where(x => x.Orders.Count() > 0)
                .Select(c => new
                {
                    customerId = c.CustomerID,
                    orderDate = c.Orders.Select(o => o.OrderDate).First()
                });

            ObjectDumper.Write($"list of customers with date of first order.");
            foreach (var customer in customers)
            {
                ObjectDumper.Write($"Customer: {customer.customerId}, first order date: {customer.orderDate}");
            }
        }

        [Category("Restriction Operators")]
        [Title("Task 005")]
        [Description("This sample returns list of customers with date of first order sorted by year, month, total order sum and customer name")]
        public void Linq005()
        {
            var customers = dataSource.Customers
                .Where(x => x.Orders.Count() > 0)
                .Select(c => new
                {
                    customerId = c.CustomerID,
                    orderDate = c.Orders.Select(o => o.OrderDate).First(),
                    totalOrderSum = c.Orders.Select(o => o.Total).Sum()
                })
                .OrderBy(c => c.orderDate.Year)
                .ThenBy(c => c.orderDate.Month)
                .ThenByDescending(c => c.totalOrderSum)
                .ThenBy(c => c.customerId);

            ObjectDumper.Write($"list of customers with date of first order sorted by year, month, total order sum and customer name.");
            foreach (var customer in customers)
            {
                ObjectDumper.Write($"Customer: {customer.customerId}, first order date year : {customer.orderDate}, total orders sum: {customer.totalOrderSum}");
            }
        }

        [Category("Restriction Operators")]
        [Title("Task 006")]
        [Description("This sample returns list of customers with nondigital postalcode or with empty region or without code operator.")]
        public void Linq006()
        {
            var customers = dataSource.Customers
                .Where(c => !long.TryParse(c.PostalCode, out long result)
                                      || string.IsNullOrEmpty(c.Region)
                                      || !c.Phone.First().Equals('('));

            ObjectDumper.Write($"list of customers with nondigital postalcode or with empty region or without code operator.");
            foreach (var customer in customers)
            {
                ObjectDumper.Write($"Customer: {customer.CustomerID}, {customer.PostalCode}, {customer.Region}, {customer.Phone}");
            }
        }

        [Category("Restriction Operators")]
        [Title("Task 007")]
        [Description("This sample returns list of products grouped by category then by available in stock and ordered by price.")]
        public void Linq007()
        {
            var products = dataSource.Products
                .GroupBy(p => p.Category)
                .Select(x => new
                {
                    category = x.Key,
                    availableProducts = x.GroupBy(p => p.UnitsInStock > 0)
                    .Select(pr => new
                    {
                        availableProduct = pr.Key,
                        products = pr.OrderBy(c => c.UnitPrice)
                    })
                });

            ObjectDumper.Write($"list of products grouped by category then by available in stock and ordered by price.");
            foreach (var product in products)
            {
                ObjectDumper.Write($"Product categories: {product.category}");
                foreach (var prod in product.availableProducts)
                {
                    ObjectDumper.Write($"Available products: {prod.availableProduct}");
                    foreach (var pr in prod.products)
                    {
                        ObjectDumper.Write($"Product price: {pr.UnitPrice}");
                    }
                }
            }
        }

        [Category("Restriction Operators")]
        [Title("Task 008")]
        [Description("This sample returns list of products grouped by price.")]
        public void Linq008()
        {
            int smallPrice = 20;
            int averagePrice = 70;
            var result = dataSource.Products
                .GroupBy(pr => pr.UnitPrice < smallPrice ? "cheap" : pr.UnitPrice < averagePrice ? "medium" : "expensive");

            ObjectDumper.Write($"list of products grouped by price.");
            foreach (var res in result)
            {
                ObjectDumper.Write($"Group name: {res.Key}");
                foreach (var product in res)
                {
                    ObjectDumper.Write($"Product price {product.UnitPrice}");
                }
            }
        }

        [Category("Restriction Operators")]
        [Title("Task 009")]
        [Description("This sample returns average income and intensity by city.")]
        public void Linq009()
        {
            var products = dataSource.Customers
                .Where(c => c.Orders.Count() > 0)
                .GroupBy(c => c.City)
                .Select(x => new
                {
                    city = x.Key,
                    averageIncome = x.Select(o => o.Orders.Select(p => p.Total).Average()).Average(),
                    averageIntensity = x.Select(o => o.Orders.Count()).Average()
                });

            ObjectDumper.Write($"list of products grouped by category then by available in stock and ordered by price.");
            foreach (var product in products)
            {
                ObjectDumper.Write($"City: {product.city}");
                ObjectDumper.Write($"Average Income: {product.averageIncome}, average Intensity: {product.averageIntensity}");
            }
        }

        [Category("Restriction Operators")]
        [Title("Task 010")]
        [Description("This sample returns statistic of client activity by Month, by Year, by Month and Year")]
        public void Linq010()
        {
            var statistics = dataSource.Customers
                .Where(c => c.Orders.Count() > 0)
                .Select(c => new
                {
                    c.CustomerID,
                    GroupByYear = c.Orders.GroupBy(x => x.OrderDate.Year).Select(g => new
                    {
                        year = g.Key,
                        activity = g.Count()
                    }),
                    GroupByMonth = c.Orders.GroupBy(x => x.OrderDate.Month).Select(g => new
                    {
                        month = g.Key,
                        activity = g.Count()
                    }),
                    GroupByMonthAndYear = c.Orders.GroupBy(x => new { x.OrderDate.Month, x.OrderDate.Year }).Select(g => new
                    {
                        month = g.Key.Month,
                        year = g.Key.Year,
                        activity = g.Count()
                    })
                });

            ObjectDumper.Write($"statistic of client activity by Month, by Year, by Month and Year.");
            foreach (var stat in statistics)
            {
                ObjectDumper.Write($"statistic of client activity by Year.");
                foreach (var year in stat.GroupByYear)
                {
                    ObjectDumper.Write($"Year: {year.year}, count of orders {year.activity}");
                }
                ObjectDumper.Write($"statistic of client activity by Month.");
                foreach (var month in stat.GroupByMonth)
                {
                    ObjectDumper.Write($"Month: {month.month}, count of orders {month.activity}");
                }
                ObjectDumper.Write($"statistic of client activity by Month and year.");
                foreach (var monthYear in stat.GroupByMonthAndYear)
                {
                    ObjectDumper.Write($"Year: {monthYear.year}, month: {monthYear.month} count of orders {monthYear.activity}");
                }
            }
        }
    }
}