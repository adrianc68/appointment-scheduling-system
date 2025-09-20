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
  assignService: `/assistant/service`,
  getAssignedServices: `/assistant/service`,

  disableAssignedService: `/scheduling/appointment/serviceOffer/disable`,
  deleteAssignedService: `/scheduling/appointment/serviceOffer/delete`,
  enableAssignedService: `/scheduling/appointment/serviceOffer/enable`,

  registerService: `/service`,
  editService: `/service`,
  disableService: `/service/disable`,
  enableService: `/service/enable`,
  deleteService: `/service/delete`,


  blockTimeRangeAppointment: `/scheduling/appointment/range/block`,
  registerAppointmentAsClient: `/scheduling/appointment/asClient`,
  registerAppointmentAsStaff: `/scheduling/appointment/asStaff`,
  confirmAppointment: `/scheduling/appointment/confirm`,
  finalizeAppointment: `/scheduling/appointment/finalize`,
  cancelAppointmentAsClient: `/scheduling/appointment/cancel/asClient`,
  cancelAppointmentAsStaff: `/scheduling/appointment/cancel/asStaff`,



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
  getScheduledAppointments: `/scheduling/appointment`,
  getScheduledAppointmentsDetails: `/scheduling/appointment/details`

};

export const ApiVersionRoute = {
  v1: "/api/v1"
};
