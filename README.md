# RateLimitApi

.net core 3.1 <br />
<br />
Implemented using Asp.net Action filters and overwriting OnActionExecuting method.<br />
<br />
The Api allocates each requester a memory cache with a expiry time and the requester can send certain number of requests within that time frame. <br />
For now it has been set to 100 requests per 1 hour (3600 seconds). 
As long as the requests have not hit the threashold, server will respond back with 201 created which means you have created a record.
There is as well a counter to count requests. When requests exceed the number, server responses with 429 too many requests and a message will be shown.

Api is integrated in a simple “Todo items” CRUD Api and is checking the HttpPost method in there.
The Api could be tested with Postman application in this way :

 ![Postman](https://user-images.githubusercontent.com/59807156/74812818-434b9900-5348-11ea-9273-1b1d034c0b6c.jpg)
 
For testing purpose you can change the number of requests and the time frame.
