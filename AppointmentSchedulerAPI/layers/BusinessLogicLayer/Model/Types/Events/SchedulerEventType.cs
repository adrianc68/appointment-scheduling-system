namespace AppointmentSchedulerAPI.layers.BusinessLogicLayer.Model.Types.Events
{
    public enum SchedulerEventType
    {
        // Appointments
        APPOINTMENT_SCHEDULED,
        APPOINTMENT_RESCHEDULED,
        APPOINTMENT_CANCELLED,
        APPOINTMENT_CONFIRMED,
        APPOINTMENT_FINISHED,
        // AvailabilityTimeSlots
        AVAILABILITY_TIME_SLOT_UPDATED,
        AVAILABILITY_TIME_SLOT_DELETED,
        AVAILABILITY_TIME_SLOT_DISABLED,
        AVAILABILITY_TIME_SLOT_ENABLED,
        // ServicesOffers
        SERVICE_OFFER_ENABLED,
        SERVICE_OFFER_DISABLED,
        SERVICE_OFFER_DELETED,
    }
}