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
 
200 - token is successfuly generated
401 - angent not found in the database