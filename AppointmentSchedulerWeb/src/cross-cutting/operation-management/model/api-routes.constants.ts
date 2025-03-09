import { EditAvailabilityTimeSlotComponent } from "../../../view/ui-process-components/edit-availability-time-slot/edit-availability-time-slot.component";

export const ApiRoutes = {
  login: `/Auth/login`,
  logout: `/Auth/logout`,
  refresh: `/Auth/refresh`,
  getAccountData: `/Auth/account/data`,
  registerClient: `/client`,
  editClient: `/client`,
  disableClient: `/client/disable`,
  enableClient: `/client/enable`,
  deleteClient: `/client/delete`,
  registerAssistant: `/assistant`,
  editAssistant: `/assistant/`,
  disableAssistant: `/assistant/disable`,
  enableAssistant: `/assistant/enable`,
  deleteAssistant: `/assistant/delete`,

  registerService: `/service`,
  editService: `/service`,
  disableService: `/service/disable`,
  enableService: `/service/enable`,
  deleteService: `/service/delete`,


  getAllAssistantServiceOffers: `/assistant/service/all`,

  registerAvailabilityTimeSlot: `/scheduling/availabilityTimeSlot`,
  editAvailabilityTimeSlot: `/scheduling/availabilityTimeSlot`,
  enableAvailabilityTimeSlot: `/scheduling/availabilityTimeSlot/enable`,
  disableAvailabilityTimeSlot: `/scheduling/availabilityTimeSlot/disable`,
  deleteAvailabilityTimeSlot: `/scheduling/availabilityTimeSlot/delete`,



  getAllNotification: `/Notification/all`,
  getUnreadNotification: `/Notification/unread`,
  markNotificationAsRead: `/Notification/read`,
  getAllClients: `/client`,
  getAllServices: `/service`,
  getAllAssistants: `/assistant`,
  getAvailabilityTimeSlots: `/scheduling/availabilityTimeSlot`,
  getAvailableServices: `/scheduling/services/available`,
  getScheduledAppointments: `/scheduling/appointment`

};

export const ApiVersionRoute = {
  v1: "/api/v1"
};
