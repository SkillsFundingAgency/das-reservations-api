Feature: AvailableDates

Scenario: Available dates are returned
	Given I have a non levy account
	And I have signed an Agreement
	When I get available dates
	Then I should get available dates back

Scenario: Available dates for Apprenticeship Unit course exclude previous month
	Given I have a non levy account
	And I have signed an Agreement
	And the course is an Apprenticeship Unit
	When I get available dates for course "1"
	Then the available dates should not include the previous month

Scenario: Available dates for non-Apprenticeship Unit course include previous month
	Given I have a non levy account
	And I have signed an Agreement
	And the course is not an Apprenticeship Unit
	When I get available dates for course "1"
	Then the first available date should be the previous month

Scenario: Available dates without courseId include previous month
	Given I have a non levy account
	And I have signed an Agreement
	When I get available dates
	Then the first available date should be the previous month