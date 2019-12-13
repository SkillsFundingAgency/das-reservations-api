Feature: AvailableDates


Scenario: Available dates are returned
	Given I have a non levy account
	And I have signed an Agreement
	When I get available dates
	Then I should get available dates back