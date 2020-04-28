Feature: ValidateReservation
	In order to determine if a reservation can be used in a commitment
	As a employer or provider
	I would like to validate date and course against an existing reservation

Scenario: Validate existing reservation inside reservation window
Given I have a non levy account 
And I have the following existing reservation: 
| StartDate  | CourseId |
| 2019-07-01 | 1        |
When I validate the reservation against the following commitment data:
| StartDate  | CourseId |
| 2019-07-01 | 1        |
Then no validation errors are returned 

Scenario: Validate existing reservation before the reservation window
Given I have a non levy account 
And I have the following existing reservation: 
| StartDate  | ExpiryDate | CourseId |
| 2019-07-01 | 2019-09-30 | 1        |
When I validate the reservation against the following commitment data:
| StartDate  | CourseId |
| 2019-06-01 | 1        |
Then validation errors are returned 

Scenario: Validate existing reservation after the reservation window
Given I have a non levy account 
And I have the following existing reservation: 
| StartDate  | ExpiryDate | CourseId |
| 2019-07-01 | 2019-09-30 | 1        |
When I validate the reservation against the following commitment data:
| StartDate  | CourseId |
| 2019-10-01 | 1        |
Then validation errors are returned 

Scenario: Validate existing reservation inside of reservation window with invalid course
Given I have a non levy account 
And the following rule exists:
| CreatedDate | ActiveFrom | ActiveTo   | CourseId |
| 2019-07-01  | 2019-07-01 | 2019-09-30 | 1        |
And I have the following existing reservation: 
| StartDate  | ExpiryDate | CourseId |
| 2019-07-01 | 2019-09-30 | 1        |
When I validate the reservation against the following commitment data:
| StartDate  | CourseId |
| 2019-07-01 | 1        |
Then validation errors are returned
	
Scenario: Validate reservation choosing framework
Given I have a non levy account
And I have the following existing reservation:
	| StartDate  | ExpiryDate | CourseId |
	| 2019-07-01 | 2019-09-30 | 1        |
When I validate the reservation against the following commitment data:
	| StartDate  | CourseId |
	| 2019-08-01 | 1-1-1    |
Then validation errors are returned

Scenario: Validate reservation that has been status of change
	Given I have a non levy account
	And I have the following existing reservation:
		| StartDate  | ExpiryDate | CourseId | Status |
		| 2019-07-01 | 2019-09-30 | 1        | 4      |
	When I validate the reservation against the following commitment data:
		| StartDate  | CourseId |
		| 2019-06-01 | 1        |
	Then no validation errors are returned 