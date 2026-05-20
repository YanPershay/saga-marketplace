import type { OrderStatus } from "../types/order";

export const terminalOrderStatuses = ["Completed", "Failed", "Cancelled"] as const;
export type TimelineStepState = "completed" | "current" | "muted";

export const orderTimelineStages = [
  {
    status: "Pending",
    label: "Order created",
    description: "The order was accepted by the gateway.",
  },
  {
    status: "AwaitingInventory",
    label: "Awaiting inventory",
    description: "Inventory reservation is in progress.",
  },
  {
    status: "InventoryReserved",
    label: "Inventory reserved",
    description: "Requested products were reserved.",
  },
  {
    status: "PaymentProcessed",
    label: "Payment processed",
    description: "Payment service confirmed the charge.",
  },
  {
    status: "ShippingScheduled",
    label: "Shipping scheduled",
    description: "Shipping service scheduled fulfillment.",
  },
  {
    status: "Completed",
    label: "Completed",
    description: "The order saga reached a successful terminal state.",
  },
] as const;

const knownOrderStatuses = [
  "Pending",
  "AwaitingInventory",
  "InventoryReserved",
  "AwaitingPayment",
  "PaymentProcessed",
  "AwaitingShipment",
  "ShippingScheduled",
  "Completed",
  "Failed",
  "Cancelled",
] as const;

const orderStatusToTimelineStage = {
  Pending: "Pending",
  AwaitingInventory: "AwaitingInventory",
  InventoryReserved: "InventoryReserved",
  AwaitingPayment: "PaymentProcessed",
  PaymentProcessed: "PaymentProcessed",
  AwaitingShipment: "ShippingScheduled",
  ShippingScheduled: "ShippingScheduled",
  Completed: "Completed",
} as const;

export function isTerminalOrderStatus(status?: OrderStatus) {
  return terminalOrderStatuses.includes(
    status as (typeof terminalOrderStatuses)[number],
  );
}

export function getOrderStatusTone(status?: OrderStatus) {
  if (status === "Completed") return "success";
  if (status === "Failed" || status === "Cancelled") return "danger";
  return "active";
}

export function getTimelineStepState(
  orderStatus: OrderStatus | undefined,
  stageStatus: (typeof orderTimelineStages)[number]["status"],
): TimelineStepState {
  if (
    !orderStatus ||
    orderStatus === "Failed" ||
    orderStatus === "Cancelled" ||
    !isKnownOrderStatus(orderStatus)
  ) {
    return "muted";
  }

  const currentStageStatus =
    orderStatusToTimelineStage[
      orderStatus as keyof typeof orderStatusToTimelineStage
    ];
  const currentIndex = orderTimelineStages.findIndex(
    (stage) => stage.status === currentStageStatus,
  );
  const stageIndex = orderTimelineStages.findIndex(
    (stage) => stage.status === stageStatus,
  );

  if (currentIndex < 0 || stageIndex < 0) return "muted";
  if (stageIndex < currentIndex) return "completed";
  if (stageIndex === currentIndex) return "current";
  return "muted";
}

export function getTimelineNotice(status?: OrderStatus) {
  if (status === "Failed") {
    return {
      label: "Failed terminal state",
      description:
        "The order saga stopped in a failed terminal state. Earlier successful progress is not inferred from this status.",
      tone: "danger" as const,
    };
  }

  if (status === "Cancelled") {
    return {
      label: "Cancelled terminal state",
      description:
        "The order saga stopped in a cancelled terminal state. Earlier successful progress is not inferred from this status.",
      tone: "danger" as const,
    };
  }

  if (status && !isKnownOrderStatus(status)) {
    return {
      label: `Unknown status: ${status}`,
      description:
        "This status is not mapped to a known saga stage, so the timeline is kept in a safe fallback state.",
      tone: "active" as const,
    };
  }

  return null;
}

function isKnownOrderStatus(status: OrderStatus) {
  return knownOrderStatuses.includes(status as (typeof knownOrderStatuses)[number]);
}
