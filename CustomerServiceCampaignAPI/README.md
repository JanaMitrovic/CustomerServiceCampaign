# Customer Service Campaign WEB API

# Project overview

The purpose of this project is to simulate real case scenario related to the implementation of a campaign to reward loyal customers.
All information about loyal customers are accessible from the FindPerson endpoint of the SOAPDemo API. Link to this endpoint is 
https://www.crcind.com/csp/samples/%25SOAP.WebServiceInvoke.cls?CLS=SOAP.Demo&OP=FindPerson and the required parameter to get customer
information is its Id. All the endpoints of this api are secured with Jwt token. Considering that, firsty user should login to get 
generated Bearer token which has to be passed as enpoints Authorization header. After agent is authorized it is possible to start the campaing,
create purchases for a specific campaign and customer, as well as to get .csv report about campaigns successful purchases and to obtain 
report with all neccessary data about those successful purchases.

# Project endpoints

## Login endpoint

This endpoint is used to generate Bearer JWT token which is used for all the api endpoints authorization. It requires email parameter
that represents the email of the agent. All the agents are predefined and thier information is stored in the database. Endpoint firsty
checks if agent with passed email exists in the database and if it does generated jwt token and returns response with its value.

URL '/authorization/login'

### request body format

```JSON
#json body
{
  "email": "string"
}
```

### response - returns token value
```JSON
#json body
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InN0cmluZyIsImV4cCI6MTY5NzYzODUwMywiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMDUiLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo0NDMwNSJ9.pehMYJo49w31eNP3XpmSTXFjQNRI5hHqWEUE4saZTjM"
}
```

### response statuses
 
* 200 - token is successfuly generated
* 401 - agent not found in the database

## Start campaign endpoint

This endpoint is used to start the campaign. It requires three parameters - company name, campaign name and campaign start date.
When endpoint is called it will calculate the end date of the campaign as 6 days after start date as campaign should last for one week.
It returns information of create campaign.

URL '/startCampaign'

### request body format

```JSON
#json body
{
  "company": "Name",
  "campaignName": "Campaign",
  "startDate": "2023-10-16T18:59:22.449Z"
}
```

### response - returns created campaign
```JSON
#json body
{
    "id": 34,
    "company": "Name",
    "campaignName": "Campaign",
    "startDate": "2023-10-16T18:59:22.449Z",
    "endDate": "2023-10-22T18:59:22.449Z"
}
```

### response statuses
 
* 201 - campaign successfuly created/started
* 400 - campaign object from request is null
* 400 - campaign object from request has id parameter which is greater that 0
* 401 - unauthorized

## Create purchase endpoint

This endpoint is used create purchase that shoud get a discount. It requires six parameters to be passed - Id of an agent
that is creating purchase (each agent can create 5 purchases per day for a specific campaign), Id of a campaign to which 
purchase relates, Customer Id that is making a purchase, purchase price, discunt that user gets for the purchase in percents 
and a purchase date (it has to be withing campaign duration). If the agent exists, has not reached his daily limit, camapign
exist as well as the customer (customer existis if it exists in SOAPDemo API) and purchase date is correct than price with
discount will be calculated and purchase will be saved. Endpoint returs created purchase.

URL '/createPurchase'

### request body format

```JSON
#json body
{
  "agentId": 2,
  "customerId": 7,
  "campaignId": 32,
  "price": 100.99,
  "discount": 20,
  "purchaseDate": "2023-10-22T19:11:18.042Z"
}
```

### response - returns created purchase
```JSON
#json body
{
    "id": 58,
    "agentId": 2,
    "customerId": 7,
    "campaignId": 32,
    "price": 100.99,
    "discount": 20.0,
    "priceWithDiscount": 80.792,
    "purchaseDate": "2023-10-22T19:11:18.042Z"
}
```

### response statuses
 
* 201 - campaign successfuly created/started
* 400 - purchase object from request is null
* 400 - purchase object from request has id parameter which is greater that 0
* 400 - agent does not exist
* 400 - campaign does not exist
* 400 - campaign is not active (purchase date is not withing campaign duration)
* 400 - customer does not exist
* 400 - agent passed his daily limit
* 401 - unauthorized

## Get .csv report endpoint

This endpoint is used to generate .csv report of successful purchases for a specific campaign. It requires two
parameters - first one is campaign Id and second one is current date. Current date when company requires to get
the report has to be at least one month after campaign end. If campaign with passed id exists and current date
is accessible report will be generated.

URL '/getCsvReport'

### request body format

```JSON
#json body
{
  "campaignId": 32,
  "currentDate": "2023-11-25T19:06:08.282Z"
}
```

### response - returns .csv file with requested data

Id,AgentName,AgentSurname,AgentEmail,CampaignName,Price,Discount,PriceWithDiscount,PurchaseDate,CustomerId
54,John,Smith,john@gmail.com,c,144,14,123,10/20/2023 12:29:00 PM,1
55,Mark,Kenfild,mark@gmail.com,c,1000,20,800,10/22/2023 7:11:18 PM,7
56,Mark,Kenfild,mark@gmail.com,c,100,20,80,10/22/2023 7:11:18 PM,7
57,Mark,Kenfild,mark@gmail.com,c,100,20,80,10/22/2023 7:11:18 PM,7
58,Mark,Kenfild,mark@gmail.com,c,100,20,80,10/22/2023 7:11:18 PM,7

### response statuses
 
* 200 - file is created
* 400 - campaign with passes id does not exist
* 400 - current date is not at least one month after campaign end
* 404 - no purchases found for desired campaign
* 401 - unauthorized

## Show report data

This endpoint is used to show data from the .csv report with customer data obtained from the SOAPDemo API.
It requires file filed through which .csv file is passed. As a response this endpoint returs all the report
data with data of the customer for each purchase.

URL '/showReportData'

### request body format

successfulPurchases.csv

### response - returs list of successful purchases with customer data
```JSON
#json body
[
    {
        "id": 60,
        "agentName": "Mark",
        "agentSurname": "Kenfild",
        "agentEmail": "mark@gmail.com",
        "campaignName": "Campaign",
        "price": 100,
        "discount": 20,
        "priceWithDiscount": 80,
        "purchaseDate": "2023-10-22T19:11:18",
        "customer": {
            "name": "Ubertini,Natasha G.",
            "ssn": "986-47-7645",
            "dob": "1934-09-15T00:00:00",
            "age": 89
        }
    },
    {
        "id": 61,
        "agentName": "Mark",
        "agentSurname": "Kenfild",
        "agentEmail": "mark@gmail.com",
        "campaignName": "Campaign",
        "price": 2000,
        "discount": 20,
        "priceWithDiscount": 1600,
        "purchaseDate": "2023-10-22T19:11:18",
        "customer": {
            "name": "Ubertini,Natasha G.",
            "ssn": "986-47-7645",
            "dob": "1934-09-15T00:00:00",
            "age": 89
        }
    }
]
```

### response statuses
 
* 201 - list of successful purchases returned
* 400 - file object is null
* 400 - file is empty
* 400 - customer from the purchase does not exist
* 401 - unauthorized
