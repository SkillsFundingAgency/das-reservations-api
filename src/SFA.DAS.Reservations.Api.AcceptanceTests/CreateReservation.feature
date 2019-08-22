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



