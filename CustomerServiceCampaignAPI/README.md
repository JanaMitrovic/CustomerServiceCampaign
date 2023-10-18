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
  "company": "string",
  "campaignName": "string",
  "startDate": "2023-10-18T13:34:17.064Z"
}
```

### response - returns token value
```JSON
#json body
{
  "id": 0,
  "company": "string",
  "campaignName": "string",
  "startDate": "2023-10-18T13:34:17.091Z",
  "endDate": "2023-10-18T13:34:17.091Z"
}
```

### response statuses
 
* 201 - campaign successfuly created/started
* 400 - campaign object from request is null
* 400 - campaign object from request has id parameter which is greater that 0
* 401 - unauthorized