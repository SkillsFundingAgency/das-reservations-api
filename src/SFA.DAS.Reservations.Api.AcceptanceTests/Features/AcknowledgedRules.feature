Feature: AcknowledgedRules
	In order to be made aware of upcoming rules
	I want to be able to see them 
	And dimiss and not see ones that I've read and acknoewledged

Scenario: Acknowledge Upcoming Rules for Provider
	Given I have upcoming rules
	And I have a non levy account
	When I acknowledge a rule as being read as a provider
	Then it is not returned in the list of upcoming rules for the provider

Scenario: Acknowledge Upcoming Rules for Employer
	Given I have upcoming rules
	And I have a non levy account
	When I acknowledge a rule as being read as a employer
	Then it is not returned in the list of upcoming rules for the employer