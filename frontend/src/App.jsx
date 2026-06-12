import { Routes, Route } from 'react-router-dom';
import { ConfigProvider, theme } from 'antd';
import ruRU from 'antd/locale/ru_RU';
import MainLayout from './components/MainLayout/MainLayout';
import HomePage from './pages/HomePage/HomePage';
import ChamberFurnacePage from './pages/ChamberFurnace/ChamberFurnacePage';
import DrumDryerPage from './pages/DrumDryer/DrumDryerPage';

const appTheme = {
  token: {
    colorPrimary: '#e8590c',
    colorInfo: '#1890ff',
    colorSuccess: '#52c41a',
    colorWarning: '#faad14',
    colorError: '#f5222d',
    borderRadius: 8,
    fontFamily:
      "'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif",
    fontSize: 14,
    colorBgLayout: '#f7f5f2',
    controlHeight: 38,
  },
  components: {
    Button: {
      primaryShadow: '0 4px 12px rgba(232, 89, 12, 0.3)',
      borderRadius: 8,
      controlHeight: 40,
    },
    Card: {
      borderRadiusLG: 12,
    },
    Input: {
      borderRadius: 8,
    },
    InputNumber: {
      borderRadius: 8,
    },
    Select: {
      borderRadius: 8,
    },
    Table: {
      borderRadius: 10,
      headerBg: 'rgba(232, 89, 12, 0.04)',
    },
    Menu: {
      darkItemBg: 'transparent',
      darkSubMenuItemBg: 'transparent',
    },
    Tabs: {
      inkBarColor: '#e8590c',
      itemActiveColor: '#e8590c',
      itemSelectedColor: '#e8590c',
      itemHoverColor: '#fa8c16',
    },
  },
};

export default function App() {
  return (
    <ConfigProvider theme={appTheme} locale={ruRU}>
      <Routes>
        <Route element={<MainLayout />}>
          <Route index element={<HomePage />} />
          <Route path="chamber-furnace" element={<ChamberFurnacePage />} />
          <Route path="drum-dryer" element={<DrumDryerPage />} />
        </Route>
      </Routes>
    </ConfigProvider>
  );
}
