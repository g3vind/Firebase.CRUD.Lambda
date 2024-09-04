using Amazon.Lambda.Core;
using Google.Cloud.Firestore.V1;
using Firebase.CRUD.Models;
using Google.Cloud.Firestore;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Firebase.CRUD
{
    public class Function
    {
        private static FirestoreDb _firestoreDb;

        static Function()
        {
            string json = @"
            {
                ""type"": ""service_account"",
                ""project_id"": ""lambda-crud"",
                ""private_key_id"": ""bd85f68ac5e5276f20352a25ba6f8a25441fb97a"",
                ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCrQrfMYvUOpB5q\nkOszNfSKUjlYRpyfbtHjTnZUiOAXHFiIwDLu2BUfQqkehCb2Xk+tSvlZb4P5c/VQ\nIx4xNrKKPYjkIzuFlgJnUOHjrYex/Ii85osqiEiRmAUpQvaIhRCZw1O6pvULKXI5\nVf5A4o8VGdTpKqLUDBKS7IQtmidgCvvFrQX1llRqgVsVr2TFcR8QfbHuGe2r/MXn\nfxGF0yczje7UZjhytnlLiR6dzMPw9qHooKS+x7omcuqAeufC9ogfbxqwSN4q+tx/\n1awUx8eynbTz/csP0GpZmkobkxhwZ9FwXR77xVSuIcsfkVR8ZshI2VtzK9ZXq7d4\ndx31Ok3zAgMBAAECggEAGaxcQkL5apL2zlHgOGKXwJ6X4yBevVXPGAy8l2qXcU96\nBDIP0JdK/08ubBkygA9eUWOiYqJhfeVAup0xGv6QoEGeQlj3xMxamuNYoZlzdJ6f\nXvYOmbcpbc4ExA06SPftp6dM53e810oEygyWm2J7QZYIQVX3Z+NRa34SSIXJ7k8o\nMMa5Rpt3FdYpGfScmm4tBSSuyOGPwQpDhFhX+Z7xT6BBrylcjfoDX2CBT6QsgBMW\n7IM0zagFZEp8OspwTdqTDawV++SHbgK/PFWBjDL5s7vIf8xuJc/Dduy0e3hNjpNs\nC7/Wij9BmXDAeuWMQ+B+u651EVmwzXmD26be2BQcmQKBgQDkSq0dAOzWjlvAf8g5\nUzKZKmAw6thfoGVxruJLA2fKU/iDtaMjzeUyxgXeytt9if25mxUx9DvhaJg+w2yh\n4FcqiqDKz6Yjftp6WIQEgR/kdMO4iKyXbR12GAw1NEBas9CuemyPUVvT+9zRL5TG\nOmojALlOvivbcn97mWgHZxCpCwKBgQDADALDC/bMdDX+2d4xAibF6dK4q2G+wbsy\nBKG3zRAL9rHaFlt788T0TPIKQmcq58IYjvH2b7BkCS0QQqkDBxEbrQkbNATJltHu\ndQClQJj48SBLq52NDtN+FM9o5Q/TRrUqGI9SIQ5sZIFaaSb8XWfKV/nh2QfLE2Pi\nNCtO6ymPuQKBgBt2kFTI47T78zW5KZEb3R5n5qJ73gPC+/EtJLP5eObyKxdNJo8M\nGeJ9x/8BUF5N12QYr/gOdZWsS4RhZhQjNNk9TBKZ4Fas0AOf2mHgVsHY+iKc7oZY\n7g5i3jUYUBWZRCV5PM4Q74cU6X+QxckDBfmoAKLkZcpEf+mSjt01HN/5AoGAWp/Q\nVdY5ejWEnWSWEy5euZojU/2bvHaTLYE+Bwv/fIXHW5CdTxqzYE1HEkxPftGqqAgc\nahw+cpZZ64hpVQJqslKvL2UWtUI0goA388NN7HtssAil6kCK0L5lOayOETcWmmzI\nhQsQUVoEKHPib8hsB/IH+ul9Yfkec0oG/dC+5hECgYEAsleYDld3/Gc5uB4VtpVa\nNw4aSOGdxQEMh28fPMxmW/fcYWgTpcHtFcsTswTi+rYQOMF3AL52fmSkkDrho+IZ\nZgZRc0Jajj31L6Mj+qHz1lDYEbUailKsUAzz8iKch+WrU9U90n5ZSimUQa+UkiWS\nMhaJeLnc/JJVDjznBfzh68g=\n-----END PRIVATE KEY-----\n"",
                ""client_email"": ""firebase-adminsdk-2381g@lambda-crud.iam.gserviceaccount.com"",
                ""client_id"": ""116372282642169693962"",
                ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
                ""token_uri"": ""https://oauth2.googleapis.com/token"",
                ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
                ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-2381g%40lambda-crud.iam.gserviceaccount.com""
            }";

            var builder = new FirestoreClientBuilder
            {
                JsonCredentials = json
            };
            _firestoreDb = FirestoreDb.Create("lambda-crud", builder.Build());
        }
        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Post, "POST")]
        public async Task<ApiResponse> ProcessOrder([FromBody]Order order, ILambdaContext context)
        {
            if(order==null)
            {
                context.Logger.LogLine("Order data is required but was not provided.");
                return new ApiResponse
                {
                    success = false,
                    message = "order data is required",
                    code = 400,
                };
            }
            if (string.IsNullOrEmpty(order.OrderId) || string.IsNullOrEmpty(order.ProductName) || order.Quantity <= 0 || order.Price <= 0)
            {
                context.Logger.LogLine("Invalid order data provided.");
                return new ApiResponse
                {
                    success = false,
                    message = "Invalid order data.",
                    code = 400,
                };
            }
           
            try
            {
                var docRef = _firestoreDb.Collection("Orders").Document(order.OrderId);
                var snapshot = await docRef.GetSnapshotAsync();

                if(snapshot.Exists)
                {
                    context.Logger.LogLine($"Order with ID {order.OrderId} already exists.");
                    return new ApiResponse
                    {
                        success = false,
                        message = $"Order with ID {order.OrderId} already exists.",
                        code = 400,
                    };
                }

                var orderData = new Dictionary<string, object>
                {
                    { "OrderId", order.OrderId },
                    { "ProductName", order.ProductName },
                    { "Quantity", order.Quantity },
                    { "Price", order.Price },
                    { "OrderDate", DateTime.UtcNow }
                };

                await docRef.SetAsync(orderData);

                context.Logger.LogLine($"Order {order.OrderId} has been added successfully.");
                return new ApiResponse
                {
                    success = true,
                    message = "order has been added successfully",
                    code = 200,
                    order = order,
                };
            }
            catch(Exception ex)
            {
                context.Logger.LogLine($"Error: {ex.Message}");
                return new ApiResponse
                {
                    success = false,
                    message = "An error occurred while adding the order.",
                    code = 500 
                };
            }
            
        }

       /* [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Get,"GET")]
        public async Task<ApiResponse> GetAllOrders(ILambdaContext context)
        {
            try
            {
                var ordersCollection = _firestoreDb.Collection("Orders");
                var snapshot = await ordersCollection.GetSnapshotAsync();
                if (!snapshot.Documents.Any())
                {
                    return new ApiResponse
                    {
                        success = true,
                        message = "No orders found.",
                        code = 404,
                        order = null
                    };
                }
                var ordersData = snapshot.Documents.Select(doc => doc.ConvertTo<Order>()).ToList();
                context.Logger.LogLine($"Retrieved {ordersData.Count} orders.");
                return new ApiResponse
                {
                    success = true,
                    message = "Orders retrieved successfully.",
                    code = 200,
                    orders = ordersData
                };
            }
            catch(Exception ex)
            {
                context.Logger.LogLine($"Error: {ex.Message}");
                return new ApiResponse
                {
                    success = false,
                    message = "An error occurred while retrieving orders.",
                    code = 500,
                    order = null
                };
            }

        }*/
    }
}
