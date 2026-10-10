import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route } from "react-router";
import "./index.css";
import ImportPage from "./pages/ImportPage";
import AppLayout from "./layouts/AppLayout";
import DashboardPage from "./pages/DashboardPage.tsx";
import ActivitiesPage from "./pages/ActivitiesPage.tsx";
import AnalyticsPage from "./pages/AnalyticsPage.tsx";
import HeatmapPage from "./pages/HeatmapPage.tsx";
import SettingsPage from "./pages/SettingsPage.tsx";
import NotFoundPage from "./pages/NotFoundPage.tsx";

const queryClient = new QueryClient();

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Routes>
          <Route element={<AppLayout />}>
            <Route index element={<DashboardPage />} />
            <Route path="activities" element={<ActivitiesPage />} />
            <Route path="import" element={<ImportPage />} />
            <Route path="analytics" element={<AnalyticsPage />} />
            <Route path="heatmap" element={<HeatmapPage />} />
            <Route path="settings" element={<SettingsPage />} />
            <Route path="*" element={<NotFoundPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  </StrictMode>,
);
