Feature: GetCourses
	In order to view available courses
	As a employer or provider
	I want to be able to get all active courses


Scenario: Get Active Course 
	Given there is an active course available
	When I get courses
	Then the active course is returned


Scenario: Try to get Inactive Course 
	Given there are no active courses available
	When I get courses
	Then no courses are returned