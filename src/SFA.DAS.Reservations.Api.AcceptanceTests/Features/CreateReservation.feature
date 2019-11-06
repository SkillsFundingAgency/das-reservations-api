Feature: CreateReservation
	In order to reserve funding for my apprenticeships
	As a employer or provider
	I would like to create a reservation

Scenario: Create a reservation as an non levy employer
Given I have a non levy account
When I create a reservation for a course with a start month of August
Then a reservation with course and start month August is created

Scenario: Create a reservation as an levy employer
Given I have a levy account
When I create a reservation for a course with a start month of August
Then a reservation with course and start month August is created

Scenario: Reservation limit for non levy employers
Given I have a non levy account
And it has a reservation limit of 1
And I have an existing reservation with status Completed
When I create a reservation for a course with a start month of July
Then I have 1 reservation 

Scenario: Reservation limit only applies to non levy
Given I have a non levy account
And it has a reservation limit of 1
And I have an existing reservation with status Completed
When I create a levy reservation
Then I have 2 reservation 

Scenario: Reservation deleted for non levy employers
Given I have a non levy account
And it has a reservation limit of 1
And I have an existing reservation with status Deleted
When I create a reservation for a course with a start month of July
Then I have 2 reservation 

Scenario: Reservation expired for non levy employers
Given I have a non levy account
And it has a reservation limit of 1
And I have the following existing reservation:
| ExpiryDate | 
| 2019-01-01 |          
When I create a reservation for a course with a start month of July
Then I have 2 reservation 

Scenario: Create reservation with non levy restriction
Given I have a levy account
And there is a restriction for non-levy accounts
When I create a levy reservation
Then I have 1 reservation