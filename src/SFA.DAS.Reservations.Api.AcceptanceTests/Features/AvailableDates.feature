Feature: AvailableDates


Scenario: Available dates are returned for non eoi
	Given I have a non levy account
	And I have signed an EOI Agreement
	When I get available dates
	Then I should get EOI available dates back

Scenario: Available dates are returned for eoi
	Given I have a non levy account
	And I have not signed an EOI Agreement
	When I get available dates
	Then I should get standard available dates back