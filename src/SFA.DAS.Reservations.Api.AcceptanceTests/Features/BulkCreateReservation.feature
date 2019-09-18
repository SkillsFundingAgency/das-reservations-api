Feature: BulkCreateReservation
	In order to allow bulk creation of reservations
	As a levy employer
	I want a specified number of reservations created for my account

Scenario: Bulk reservation creation
	Given I have a levy account
	When I bulk create 10 reservations
	Then 10 levy reservations are created

Scenario: Bulk reservation creation with transfer funds
	Given I have a non levy account
	And I am receiving transfer funds
	When I bulk create 10 reservations
	Then 10 levy reservations are created
	And the transfer id is added to the 10 reservations