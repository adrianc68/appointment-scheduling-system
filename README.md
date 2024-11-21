# Project Overview
## System Name:
- `Appointment Scheduling System`
## System Description:
- `The Appointment Scheduling System aims to facilitate the management of client appointments for business X. This system should allow clients to schedule their own appointments or provide business owners the ability to schedule appointments on behalf of their clients.`
* `Ideal for managing appointment times, such as for medical offices, salons or consultations`
## System Objectives:
- `Allow clients of business X to schedule their own appointments to physically attend the business.`
- `Allow the staff of business X to schedule appointments for their clients so that they can physically attend the business.`

## System Features:

### Client Module:
1. `Register client`
2. `Delete client`
3. `Disable client`
4. `Enable client`
### Schedule Module:
1. `Schedule appointment`
2. `Edit appointment`
3. `Cancel appointment`
4. `Finalize appointment`
### Availability Module:
1. `Add availability time slot`
2. `Edit availability time slot`
3. `Delete availability time slot`
### Service Module:
1. `Register service`
2. `Edit service`
3. `Disable service`
4. `Enable service`
5. `Delete service`


## Proposed Methodology or Methods:
- `Attribute-Driven Design
### Methodology Activities:
- `Pending`


# Software Process

## Requirements

### Use Cases


![](images/any/usecasemodel.png)

![](images/any/package.png)



### Use Case Descriptions

#### Agenda Module

| Use Case                   | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
| -------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| CA-01 Schedule Appointment | - The Agenda Manager clicks the button or clicks on the agenda at a specific time  <br>- The system displays the "Schedule Appointment" window with a START TIME assigned if the agenda was clicked, and retrieves the SERVICES registered in the database  <br>- The Agenda Manager selects the required services  <br>- The system validates the information, and if valid, registers the APPOINTMENT in the database, shows a success message, and sends a notification to the ADMINISTRATOR  <br>- The administrator receives the notification and clicks the "View Appointment Details" button  <br>- The system displays the "Confirm Appointment" window with the appointment details  <br>- The administrator either modifies or leaves the details unchanged and clicks the "Confirm Appointment" button  <br>- The system validates the data, and if valid, changes the appointment status from PENDING to CONFIRMED and notifies the CLIENT of the confirmation, also sending the confirmation via email or SMS to the CLIENT's phone number |
| CA-02 Edit Appointment     | - The Agenda Manager clicks on a specific time slot in the "Schedule Appointment" window  <br>- The system displays the details of the time slot in a modal window  <br>- The Agenda Manager (Administrative Personnel) clicks the "View Details" button  <br>- The system displays the "Edit Appointment" window  <br>- The Administrative Personnel enters the new data and clicks the "Accept" button  <br>- The system validates the information, and if valid, updates the data in the database and shows a success message.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| CA-03 Cancel Appointment   | - The ADMINISTRATOR selects an appointment in the "Appointments" window  <br>- The system shows the details of the appointment in a modal window  <br>- The ADMINISTRATOR clicks the "View Details" button  <br>- The system displays the "Edit Appointment" window  <br>- The ADMINISTRATOR clicks the "Cancel Appointment" button  <br>- The system shows a confirmation window and requests the ADMINISTRATOR's password  <br>- The administrator enters the ADMINISTRATOR's password and clicks the "Confirm" button  <br>- The system verifies the information, and if valid, deletes the appointment from the database and shows a success message                                                                                                                                                                                                                                                                                                                                                                                                |
| CA-04 Finalize Appointment | - The Administrative Personnel selects an appointment in the "Appointments" window  <br>- The system shows the details in a modal window  <br>- The Administrative Personnel clicks the "View Details" button  <br>- The system displays the "Edit Appointment" window  <br>- The Administrative Personnel clicks the "Finalize Appointment" button                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     |
#### Availability Module

| Use Case                           | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| ---------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| CB-1 Assign Availability time slot | - The administrator clicks the "Assign Availability" button in the "Availability" window  <br>- The system displays the "Assign Availability" window  <br>- The administrator enters the information for the available time slot and clicks the "Schedule" button  <br>- The system validates the information, and if valid, displays a success message                                                                                                                                                           |
| CB-2Delete Availability Slot       | - The administrator selects a time slot in the "Availability" window <br>- The system shows a modal window with the details of the assigned time slot  <br>- The administrator clicks the "Delete" button  <br>- The system shows a confirmation window and requests the ADMINISTRATOR's password  <br>- The administrator enters the ADMINISTRATOR's password and clicks the "Confirm" button  <br>- The system deletes the ASSIGNED TIME SLOT from the ASSISTANT in the database and displays a success message |
| CB-3 Edit Availability             | - The administrator selects a time slot in the "Availability" window <br>- The system shows a modal window with the details of the assigned time slot  <br>- The administrator clicks the "Edit" button  <br>- The system displays the "Edit Assistant Availability" window  <br>- The administrator provides the new availability information and clicks the "Accept" button  <br>- The system validates the information, and if valid, displays a success message                                               |

#### Assistant Module

| Use Case                | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
| ----------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| CC-1 Register Assistant | - The system administrator clicks the "Register" button in the "Assistants" window  <br>- The system displays the "Register Assistant" window  <br>- The administrator provides the assistant's data and selects the assistant's services, then clicks the "Add" button  <br>- The system displays the "Add Services" window  <br>- The administrator selects the services and clicks the "Accept" button  <br>- The system adds the selected services to the "Assigned Services to Assistant" table  <br>- The administrator clicks the "Register" button  <br>- The system verifies the data and proceeds to register the new assistant in the database and displays a success message                                |
| CC-2 Edit Assistant     | - The system administrator clicks the "View Details" button in the "Assistants" table in the "Assistants" window  <br>- The system displays the "Edit Assistant" window  <br>- The administrator provides the new assistant's data and selects the assistant's services, then clicks the "Add" button  <br>- The system displays the "Add Services" window  <br>- The administrator selects the services and clicks the "Accept" button  <br>- The system adds the selected services to the "Assigned Services to Assistant" table  <br>- The administrator clicks the "Register" button  <br>- The system verifies the data and proceeds to modify the assistant's data in the database and displays a success message |
| CC-3 Disable Assistant  | - The system administrator clicks the "View Details" button in the "Assistants" table in the "Assistants" window  <br>- The system displays the "Edit Assistant" window  <br>- The administrator clicks the "Disable" button  <br>- The system checks if the assistant's status is ENABLED and, if valid, changes the assistant's status to DISABLED and displays a success message; if not, an error message is displayed                                                                                                                                                                                                                                                                                              |
| CC-4 Enable Assistant   | - The system administrator clicks the "View Details" button in the "Assistants" table in the "Assistants" window  <br>- The system displays the "Edit Assistant" window  <br>- The administrator clicks the "Enable" button  <br>- The system checks if the assistant's status is DISABLED and, if valid, changes the assistant's status to ENABLED and displays a success message; if not, an error message is displayed                                                                                                                                                                                                                                                                                               |

#### Services Module

| Use Case              | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
| --------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| CD-1 Register Service | - The system administrator clicks the "Register" button in the "Services" window  <br>- The system displays the "Register Service" window  <br>- The administrator provides the new service information and clicks the "Accept" button  <br>- The system checks that the information is not duplicated, and if valid, adds the new service to the database and displays a success message                                                                                                    |
| CD-2 Disable Service  | - The system administrator clicks the "View Details" button for a service in the "Services" table  <br>- The system displays the "Edit Service" window  <br>- The administrator clicks the "Disable" button  <br>- The system checks that the service is in the ENABLED state and, if valid, changes the status to DISABLED in the database and displays a success message                                                                                                                   |
| CD-3 Enable Service   | - The system administrator clicks the "View Details" button for a service in the "Services" table  <br>- The system displays the "Edit Service" window  <br>- The administrator clicks the "Enable" button  <br>- The system checks that the service is in the DISABLED state and, if valid, changes the status to ENABLED in the database and displays a success message                                                                                                                    |
| CD-4 Delete Service   | - The system administrator clicks the "View Details" button for a service in the "Services" table  <br>- The system displays the "Edit Service" window  <br>- The administrator clicks the "Delete" button  <br>- The system displays a confirmation window and requests the ADMINISTRATOR's password  <br>- The administrator provides the ADMINISTRATOR's password and clicks the "Confirm" button  <br>- The system deletes the service from the database and displays a success message. |
| CD-5 Edit Service     | - The system administrator clicks the "View Details" button for a service in the "Services" table  <br>- The system displays the "Edit Service" window  <br>- The administrator provides the new service information and clicks the "Accept" button  <br>- The system verifies the data and proceeds to modify the information in the database and displays a success message.                                                                                                               |
#### Client Module

| Use Case             | Description                                                                                                                                                                                                                                                                                                                                                                                                                                               |
| -------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| CE-1 Register Client | - The system administrator provides the required information for the clients in a registration window.  <br>- The system checks that the client information to be registered is not duplicated, and if valid, adds the information to the database and updates the client table. If not, it displays a message about duplicate information                                                                                                                |
| CE-2 Disable Client  | - The system administrator selects a client from the client table and clicks the "Disable" button  <br>- The system displays the "Edit Client" window  <br>- The administrator clicks the "Disable Client" button  <br>- The system checks that the client is not currently disabled in the database and, if valid, changes the client's status to DISABLED and displays a success message. If not, it displays an error message.                         |
| CE-3 Enable Client   | - The system administrator selects a client from the client table and clicks the "View Details" button  <br>- The system displays the "Edit Client" window  <br>- The administrator clicks the "Enable Client" button  <br>- The system checks that the client is not currently enabled in the database and, if valid, changes the client's status to ENABLED and displays a success message. If not, it displays an error message.                       |
| CE-4 Delete Client   | - The system administrator clicks the "View Details" button for a client  <br>- The system displays the "Edit Client" window  <br>- The administrator clicks the "Delete" button  <br>- The system displays a confirmation window and requests the ADMINISTRATOR's password  <br>- The administrator provides the password and clicks the "Delete" button  <br>- The system deletes the selected client from the database and displays a success message. |

### Functional Requirements

- The system should allow selecting a time range and block that slot for at least 2 minutes before the appointment is canceled. The selected time range should have a maximum duration of 3 hours.
- The system should only allow scheduling a maximum of one appointment per user.
- The system must notify the administrator, client or assistant within 10-15 seconds when an appointment is created, finalized or canceled.
- The system must send automated reminders (via email or SMS) to users (clients, assistants, or administrators) about upcoming appointments at predefined intervals (30 minutes before the appointment).
- The system should keep a record of all past appointments and their statuses (completed or canceled) and allow users to view their appointment history.
- The system should allow clients to cancel appointments before the appointment is started.
- The system should automatically detect scheduling conflicts and prevent them (like race condition).
- The system must allow up to 10 minutes of overlap between appointments.

### Constraints

| ID    | Constraint                                                                                                                                                 |
| ----- | ---------------------------------------------------------------------------------------------------------------------------------------------------------- |
| CON-1 | Users must interact with the system through a web browser in different platforms (Windows, OSX, and Linux and different devices like computers or mobiles) |
| CON-2 | Code must be hosted on a proprietary Git-based platform like Github                                                                                        |
| CON-3 | Future support for mobile clients like IOS and Android                                                                                                     |
| CON-4 | A minimum of 50 simultaneous users must be supported                                                                                                       |

### Concerns

| ID    | Concern                                          |
| ----- | ------------------------------------------------ |
| CRN-1 | Establish an overall initial system architecture |
| CRN-2 | Avoid introducing technical debt                 |
| CRN-3 | Set up a continuous deployment infrastructure    |

### Quality Attributes

1. The system must be operational and accessible during working hours, including weekends (availability).
2. The system must support data encryption (especially for passwords and bank account information) to protect against unauthorized access (security).
3. The system must be capable of adding new features within a maximum of 2 months (modifiability).
4. The system should support multiple languages for a diverse user base, depending on the geographic region (internationalization, usability).
5. The system must support at least 500 users concurrently querying the prices of services and should be capable of handling up to 1,000 without decreasing average latency by more than 20%. (Performance)
6. The system must allow integration with external system, such as messaging and email services. (Interoperability)
7. The system must support integration testing independently of external systems (Testability)
8. The system must validate user credentials against an Identity User Service, and once logged in, the user can only access the actions they are authorized to perform. (Security)
9. The system must be integrad with a continuous deployment pipeline to automatically deploy new changes to production after successful testing. (Reliability)

Quality Attribute Scenarios

| ID  | Quality Attribute | Scenario                                                                                                                                                                                            | Associated Use Case |
| --- | ----------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------- |
| 1   | Availability      | The system must be operational and accessible during working hours, including weekends, with an uptime of 99.5% or higher.                                                                          | All                 |
| 2   | Security          | The system must use strong data encryption protocols for sensitive information, such as passwords and bank account details, to protect against unauthorized access and breaches.                    | ---                 |
| 3   | Modifiability     | The system must support the addition of new features within a maximum of 2 months, including development, testing and deployment phases.                                                            | All                 |
| 4   | Usability         | The system should support multiple languages to accommodate a diverse user base, with localization depending on the geographic region of users.                                                     | All                 |
| 5   | Performance       | The system must support at least 500 users concurrently querying the prices of services and should be capable of handling up to 1,000 without decreasing average latency by more than 20%.          | ---                 |
| 6   | Interoperability  | The system must support integration with external system, such as messaging and email services ensuring communication is reliable and occurs within 10 seconds of triggered events.                 | All                 |
| 7   | Testability       | The system must support integration testing independently of external systems by using stubs                                                                                                        | All                 |
| 8   | Security          | The system must validate user credentials against an Identity User Service, ensuring that once logged in, users can only access the actions and data they are authorized to use or view             | All                 |
| 9   | Reliability       | The system must ensure automated, seamless deployment of new features and bug fixes to production, with a maximum of downtime of 5 minutes per deployment and no rollback incidents in 95% of cases | All                 |


## Attribute-Driven Design (ADD)

The objective of starting an Attribute-Driven Design process is to establish a solid foundation that supports the system's quality attribute requirements, accommodates future changes or feature additions, and acts as a guide for development teams, ensuring a shared understanding of the system's structure and its components.

### ADD Step 1. Review Inputs

**Task**: Identify which requirement will be considered as architectural drivers.


| Scenario ID           | Importance to the Customer | Difficulty of Implementation |
| --------------------- | -------------------------- | ---------------------------- |
| QA-1 Availability     | High                       | High                         |
| QA-9 Reliability      | High                       | High                         |
| QA-6 Interoperability | High                       | Medium                       |
| QA-3 Modifiability    | High                       | Medium                       |
| QA-2, QA-8 Security   | High                       | Medium                       |
| QA-5 Performance      | Medium                     | High                         |
| QA-7 Testability      | Medium                     | Medium                       |
| QA-4 Usability        | Low                        | Medium                       |

| Category                        | Details                                                                                                                                                                                                                                                                                                                                                             |
| ------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Design purpose                  | The purpose of the design activity is to make initial decisions to support the construction of the system from scratch.                                                                                                                                                                                                                                             |
| Primary functional requirements | CE-1 Register client — It establishes a basis for other use cases<br>CC-1 Register assistant — It establishes a basis for other use cases<br>CD-1 Register service — It establishes a basis for other use cases<br>CB-1 Assign availability time slot — It establishes a basis for other use cases<br>CA-1 Schedule appointment — It supports the core business<br> |
|                                 | From this list, QA-1, QA-9, QA-6, QA-3 are selected as primary drivers.                                                                                                                                                                                                                                                                                             |
| Constraints                     | All of the previously constraints are included as drivers                                                                                                                                                                                                                                                                                                           |
| Architectural concerns          | All of the previously concerns associated with the system are included as drivers                                                                                                                                                                                                                                                                                   |

### Iteration 1: Establishing an Overall System Structure
#### Step 2: Establishing Iteration Goal by Selecting Drivers

The iteration goal is to establish an initial overall structure for the system considering the drivers that influence the general structure of the system:
* QA-9 Reliability
* QA-1 Availability
* QA-2, QA-8 Security
* CON-1 Users must interact with the system through a web browser in different platforms (Windows, OSX, and Linux and different devices like computers or mobiles)
* CRN-2 Avoid introducing technical debt
* CRN-1 Establishing an overall system architecture


![](images/any/context_diagram.png)
#### Step 3: Choose Elements of the System to Refine

The iteration goal is to achieve the architectural concern CRN-1 and refine the entire Appointment Scheduling System. In this case, refinement is performed through decomposition.

#### Step 4: Choose One or More Design Concepts That Satisfy the Selected Drivers


![](images/any/designconcepts.png)

In Greenfield system within mature domains, the design concepts that must be selected include reference architecture and deployment patterns and externally developed components. The table below represents design concepts along with their candidates:

| Design concepts                 | Candidates                                                                                                                  |
| ------------------------------- | --------------------------------------------------------------------------------------------------------------------------- |
| Reference architectures         | -Mobile applications<br>-Web Applications<br>-Service Application<br>-Rich Internet Application<br>-Rich Client Application |
| Deployment Patterns             | -4 tier<br>-3 tier<br>-2 tier                                                                                               |
| Externally Developed Components | ---                                                                                                                         |


| Design Decision                 | Rationale                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             | Reference                      |
| ------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------ |
| Mobile Application              | A mobile application allows **clients and assistants** to interact with the system on the go, enhancing **availability** by ensuring access anywhere and anytime, even when they are away from a desktop.<br><br>                                                                                                                                                                                                                                                                                                                                     | CON-1, CON-3, <br>QA-1         |
| Rich Internet Application (RIA) | Provides a more interactive and responsive experience than traditional web apps, with reduced dependency on the server. RIAs can be easily updated because the logic and interface are managed centrally on the server, but the rich client-side interactivity allows for a smoother experience. Modifications on the client side can be done independently of the server-side changes, improving **modifiability**. Once loaded, an RIA can work with minimal server communication, improving the performance and responsiveness of the application. | CON-2, QA-6, QA-3              |
| Service Application             | A service application (API Layer) encapsulates core functionality and ensures **reliability** through modular design. This separation reduces the chance of failure in one part affecting the whole system. It supports **scalability** and **reuse** of services for multiple clients like mobile, web, and CLI. It does not provide a user interface but rather expose services that are consumed by other applications.                                                                                                                            | CON-1, CON-3, QA-9             |
| 3 tier                          | Provides a simple, well-established separation of concerns between presentation, business logic, and data layers, which enhances reliability, scalability, maintainability, and cost-effectiveness without introducing unnecessary complexity.                                                                                                                                                                                                                                                                                                        | CON-1, CON-3, QA-9, QA-3, QA-6 |


**Discarded Alternatives**

| Design Decision | Rationale                                                                                                                                                                                                                                                                                                                                                                                                             |
| --------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Web Application | Typically, web applications are less interactive and rely heavily on page reloads for updates, which can make the user experience feel slower and less responsive. Standard web apps can suffer from performance issues, particularly when dealing with high-frequency updates or large amounts of data. Each user interaction may require a round-trip to the server, leading to latency and slower user experience. |
| 4 tier          | Introduces an additional layer that increases operational complexity and overhead, suitable for complex systems with advanced routing, traffic management, or microservices needs.                                                                                                                                                                                                                                    |

The following figures represent the reference architecture and deployment pattern that i have used as the foundation for gathering design concepts and making design decisions

**3-tier deployment pattern**

![](images/any/reference_architecture.png)


**Rich Internet Application (RIA)**
![](images/any/richinterneapplicationreferencearc.png)

**Service Application** 

![](images/any/serviceapplicationreferencearc.png)

**Mobile Client Application**

![](images/any/mobileclientappreferencearch.png)

#### Step 5: Instantiate Architectural Elements, Allocate Responsibilities, and Define Interfaces