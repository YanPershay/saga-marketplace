import { Navigate, Route, Routes } from "react-router-dom";
import { AppLayout } from "./components/layout/AppLayout";
import { CartPage } from "./pages/CartPage";
import { OrderStatusPage } from "./pages/OrderStatusPage";
import { ProductsPage } from "./pages/ProductsPage";

export default function App() {
  return (
    <Routes>
      <Route element={<AppLayout />}>
        <Route index element={<Navigate to="/products" replace />} />
        <Route path="/products" element={<ProductsPage />} />
        <Route path="/cart" element={<CartPage />} />
        <Route path="/orders/:orderId" element={<OrderStatusPage />} />
      </Route>
    </Routes>
  );
}
