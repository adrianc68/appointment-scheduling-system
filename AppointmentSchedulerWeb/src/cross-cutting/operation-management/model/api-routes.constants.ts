// api-routes.constants.ts
export const ApiRoutes = {
  login: `/Auth/login`,
  logout: `/Auth/logout`,
  refresh: `/Auth/refresh`,
  getAccountData: `/Auth/account/data`,
  registerClient: `/client`,
  getAllNotification: `/Notification/all`,
  getUnreadNotification: `/Notification/unread`,
  markNotificationAsRead: `/Notification/read`,

};

export const ApiVersionRoute = {
  v1: "/api/v1"
};
