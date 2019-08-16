Feature: CreateReservation
	In order to reserve funding for my apprenticeships
	As a employer or provider
	I would like to create a reservation



Scenario: Create a reservation as an employer
Given a course name baker level 2 has been added to the course list
When I create a reservation for the baker level 2 course with a start date of August
Then a reservation with course baker level 2 and start date August is created



