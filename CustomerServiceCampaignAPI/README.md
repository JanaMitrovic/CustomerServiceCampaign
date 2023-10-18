# Customer Service Campaign WEB API

# Project overview

The purpose of this project is to simulate real case scenario related to the implementation of a campaign to reward loyal customers.
All information about loyal customers are accessible from the FindPerson endpoint of the SOAPDemo API. Link to this endpoint is 
https://www.crcind.com/csp/samples/%25SOAP.WebServiceInvoke.cls?CLS=SOAP.Demo&OP=FindPerson and the required parameter to get customer
information is its Id. All the endpoints of this api are secured with Jwt token. Considering that, firsty user should login to get 
generated Bearer token which has to be passed as enpoints Authorization header. After agent is authorized it is possible to start the campaing,
create purchases for a specific camapign and customer, as well as to get .csv report about campaigns successful purchases and to obtain 
report with all neccessary data about those successful purchases.