Feature: ValidateReservation
	In order to determine if a reservation can be used in a commitment
	As a employer or provider
	I would like to validate a reservation

Scenario: Validate existing reservation
Given I have a non levy account 
And a reservation for a course in July 
When I validate the reservation against a date inside the reservation window 
And with a course on the reservation 
Then no validation errors are returned 

Scenario: Validate existing reservation outside of reservation window
Given I have a non levy account 
And a reservation for a course in July 
When I validate the reservation against a date outside the reservation window 
And with a course on the reservation 
Then validation errors are returned 

Scenario:  existing reservation inside of reservation window with invalid course
Given I have a non levy account 
And a reservation for a course in July 
When I validate the reservation against a date inside the reservation window 
And with a course that has a restriction in place 
Then validation errors are returned 
