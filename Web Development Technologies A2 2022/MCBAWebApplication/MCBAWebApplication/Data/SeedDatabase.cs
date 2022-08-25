using System;
using MCBAWebApplication.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCBAWebApplication.Data;

public static class SeedDatabase {
    public static void Initialize(IServiceProvider serviceProvider) {
        var context = serviceProvider.GetRequiredService<McbaWebContext>();

        //Look for customers
        if (context.Customers.Any())
            return;

        const string Url = "https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/";

        //Contact webservice
        using var client = new HttpClient();
        var json = client.GetStringAsync(Url).Result;

        //Convert Json into objects
        var objData = (JArray)JsonConvert.DeserializeObject(json);

        dynamic jObject = new JObject();

        foreach(var item in objData) {
            context.Customers.Add(
                new Customer {
                    CustomerID = item.Value<int>("CustomerID"),
                    Name = item.Value<string>("Name"),
                    Address = item.Value<string>("Address"),
                    Suburb = item.Value<string>("City"),
                    Postcode = item.Value<string>("PostCode")
                });

            var accountArray = new JArray(item.Value<JArray>("Accounts"));

            foreach(var account in accountArray) {
                AccountType accountType = new AccountType();
                if (account.Value<String>("AccountType") == "S")
                    accountType = AccountType.Saving;
                else
                    accountType = AccountType.Checking;

                var transactionArray = new JArray(account.Value<JArray>("Transactions"));

                decimal balance = 0.0m;

                foreach( var transaction in transactionArray) {
                    context.Transactions.Add(
                        new Transaction {
                            TransactionType = TransactionType.Deposit,
                            AccountNumber = account.Value<int>("AccountNumber"),
                            Amount = transaction.Value<decimal>("Amount"),
                            Comment = transaction.Value<string>("Comment"),
                            TransactionTimeUTC = transaction.Value<DateTime>("TransactionTimeUtc")
                        });

                    balance += transaction.Value<decimal>("Amount");
                }
                
                context.Accounts.Add(
                    new Account {
                        AccountNumber = account.Value<int>("AccountNumber"),
                        AccountType = accountType,
                        CustomerID = item.Value<int>("CustomerID"),
                        Balance = balance
                    });
            }

            // Issues getting this line to work, try to resolve it later
            //var loginArray = new JArray(item.Value<JArray>("Login"));

            context.Logins.Add(
                new Login {
                    LoginID = item["Login"].Value<string>("LoginID"),
                    CustomerID = item.Value<int>("CustomerID"),
                    PasswordHash = item["Login"].Value<string>("PasswordHash")
                });
        }

        //Adding a payee for the sake of testing the billpay features
        context.Payees.Add(
            new Payee {
                Name = "Bills R Us",
                Address = "123 Example St",
                Suburb = "Fakeville",
                Postcode = "0000",
                Phone = "04 1111 1111"
            });

        context.SaveChanges();
    }
}
