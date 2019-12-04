Feature: Check account auto reservation status
	In order to be able to auto create reservations
	as a levy or a non levy employer
	I want to check auto reservation status

Scenario: Check auto reservation status
	Given I have a levy account
	When I get the account reservation status
	Then I am allowed to auto create reservations

Scenario: Check auto reservation status non levy
	Given I have a non levy account
	When I get the account reservation status
	Then I am not allowed to auto create reservations

Scenario: Check auto reservation status as a non levy transfer receiver
	Given I have a non levy account
	And I have transfer funds
	When I get the account reservation status
	Then I am allowed to auto create reservations