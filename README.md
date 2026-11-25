
# das-reservations-api

## Build Status

![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/Manage%20Funding/das-reservations-api?branchName=master)

## Requirements

DotNet Core 2.2 and any supported IDE for DEV running.

## About

The reservations API is responsible for creating and retrieving reservations at an account level. The reservation guarantees you to an apprenticeship starting in a defined start and end period, using any restrictions that are currently in place.

## Local running

You are able to run the API using an in memory database by doing the following:

- Clone repository
- Set **Environment** to **DEV** in appsettings.json and run

You can also run it using **LOCAL** which will require a SQL instance and also Azure Storage to run in this mode

- Clone repository
- Publish `SFA.DAS.Reservations.Database` project to SQL instance
- Create table in Azure Storage called Configuration, and add an extra column called **Data**. Set partitionkey to `LOCAL`, Rowkey to `SFA.DAS.Reservations.API_1.0`, then for the Data column add the following:
- Run  Elastic Search running on docker - there is a [docker compose file](https://github.com/SkillsFundingAgency/das-reservations-jobs/tree/master/docker) which will setup ElasticSearch and Kibana for you
```
{   
	"Reservations": {    
		"ConnectionString": "Data Source=.;Initial Catalog=SFA.DAS.Reservations;Integrated Security=True;Pooling=False;Connect Timeout=30",
		"ExpiryPeriodInMonths": 6   
		},   
	"AzureAd": {     
		"Authority": "AzureADAuthorityUrl",     
		"ClientId": "AzureClientId",     
		"IdentifierUri": "IdentifierUri"   
		} 
}
```
 *if you are running on an environment other than LOCAL or DEV you will need to supply configuration for AzureAd* 
 

## Authorization

The API uses AzureAD for authentication. When running in DEV or LOCAL modes, the authentication attribute is not added. If you do enable authentication you will need to add the ```Authorization Bearer [TOKEN]``` header attribute to all requests. 


## Endpoints

### Create Reservation

**POST**
```
/api/accounts/{accountId}/reservations
```
**Body**
```
{
	"Id" : "03f6654f-78b9-4ad1-899e-a0077d7b986e",
	"StartDate" : "2019-8-02T00:00:00",
	"AccountId": 11112,
	"AccountLegalEntityName": "My Account Name"
}
```
**Additional parameters**
```
{
	"CourseId" : "1-4-123",
	"ProviderId" : 53,
	"LegalEntityAccountId" : 857694
}
```

- Id - Guid
- StartDate - DateTime
- AccountId - long
- CourseId - string (Must exist as an Id in the ```api/courses``` request)
- ProviderId - uint
- LegalEntityAccountId - long
- AccountLegalEntityName - string

**Response**
```
{
    "id": "03f6654f-78b9-4ad1-899e-a0077d7b986f",
    "accountId": 11112,
    "isLevyAccount": false,
    "createdDate": "2019-04-01T08:31:31.9385526Z",
    "startDate": "2019-08-02T00:00:00",
    "expiryDate": "2020-02-29T00:00:00",
    "isActive": true,
    "course": {
        "courseId": "1-4-123",
        "title": "Network engineer",
        "level": "4",
        "type": 0
    },
    "rules": [
        {
            "id": 1,
            "createdDate": "2019-01-24T19:54:32.533",
            "activeFrom": "2019-01-02T19:54:32.533",
            "activeTo": "2019-05-24T19:54:32.533",
            "courseId": "1-43-2",
            "restriction": 1,
            "course": {
                "courseId": "1-43-2",
                "title": "Baker",
                "level": "1",
                "type": 1
            }
        }
    ],
    "status": 0,
    "providerId": 53,
    "legalEntityAccountId": 857694,
    "accountLegalEntityName": "My Account Name"
}
```
Additionaly the location to the resource is returned

### Get Reservation
**GET**
`/api/reservations/{id}`

- Id - Guid

**Response**
```
{
    "id": "03f6654f-78b9-4ad1-899e-a0077d7b986f",
    "accountId": 11112,
    "isLevyAccount": false,
    "createdDate": "2019-04-01T08:31:31.9385526Z",
    "startDate": "2019-08-02T00:00:00",
    "expiryDate": "2020-02-29T00:00:00",
    "isActive": true,
    "course": {
        "courseId": "1-4-123",
        "title": "Network engineer",
        "level": "4",
        "type": 0
    },
    "rules": [
        {
            "id": 1,
            "createdDate": "2019-01-24T19:54:32.533",
            "activeFrom": "2019-01-02T19:54:32.533",
            "activeTo": "2019-05-24T19:54:32.533",
            "courseId": "1-43-2",
            "restriction": 1,
            "course": {
                "courseId": "1-43-2",
                "title": "Baker",
                "level": "1",
                "type": 1
            }
        }
    ],
    "status": 0,
    "providerId": 53,
    "legalEntityAccountId": 857694,
    "accountLegalEntityName": "My Account Name"
}
```


### Get Account Reservations
**GET**
`/api/accounts/{accountId}/reservations`

- AccountId - long

**Response**
```
[{
    "id": "03f6654f-78b9-4ad1-899e-a0077d7b986f",
    "accountId": 11112,
    "isLevyAccount": false,
    "createdDate": "2019-04-01T08:31:31.9385526Z",
    "startDate": "2019-08-02T00:00:00",
    "expiryDate": "2020-02-29T00:00:00",
    "isActive": true,
    "course": {
        "courseId": "1-4-123",
        "title": "Network engineer",
        "level": "4",
        "type": 0
    },
    "rules": [
        {
            "id": 1,
            "createdDate": "2019-01-24T19:54:32.533",
            "activeFrom": "2019-01-02T19:54:32.533",
            "activeTo": "2019-05-24T19:54:32.533",
            "courseId": "1-43-2",
            "restriction": 1,
            "course": {
                "courseId": "1-43-2",
                "title": "Baker",
                "level": "1",
                "type": 1
            }
        }
    ],
    "status": 0,
    "providerId": 53,
    "legalEntityAccountId": 857694,
    "accountLegalEntityName": "My Account Name"
}]
```
### Get Rules
**GET**
`/api/rules`

**Response**
```
[
    {
        "id": 1,
        "createdDate": "2019-01-24T19:54:32.533",
        "activeFrom": "2019-01-02T19:54:32.533",
        "activeTo": "2019-05-24T19:54:32.533",
        "courseId": "1-43-2",
        "restriction": 1,
        "course": {
            "courseId": "1-43-2",
            "title": "Baker",
            "level": "1",
            "type": 1
        }
    }
]
```


### Get Courses
**GET**
`/api/courses`

**Response**
```
{
    "courses": [
        {
            "courseId": "1",
            "title": "Network engineer",
            "level": "4",
            "type": 0
        }]
}
```

