using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.SQS;
using FastFoodPayment.Endpoints;
using FastFoodPayment.Logger;
using FastFoodPayment.Repositories;
using FastFoodPayment.SqsQueues;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

builder.Services.AddMemoryCache();

string accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_DYNAMO");
string secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY_DYNAMO");

AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);

var sqsClient = new AmazonSQSClient(credentials, RegionEndpoint.USEast1);

builder.Services.AddSingleton(sqsClient);
builder.Services.AddSingleton<SqsLogger>();
builder.Services.AddSingleton<SqsProduction>();

var clientConfig = new AmazonDynamoDBConfig();
clientConfig.RegionEndpoint = Amazon.RegionEndpoint.USEast1;
builder.Services.AddSingleton<IAmazonDynamoDB>(_ => new AmazonDynamoDBClient(credentials, clientConfig));
builder.Services.AddSingleton<PaymentRepository>();
var app = builder.Build();



app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.RegisterPaymentsEndpoints();

app.Run();
