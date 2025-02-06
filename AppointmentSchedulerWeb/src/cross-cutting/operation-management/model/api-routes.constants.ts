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
