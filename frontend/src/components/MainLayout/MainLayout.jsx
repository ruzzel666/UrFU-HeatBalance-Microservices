import { useState } from 'react';
import { Layout, Menu, Typography, Button } from 'antd';
import {
  HomeOutlined,
  FireOutlined,
  ExperimentOutlined,
  MenuFoldOutlined,
  MenuUnfoldOutlined,
} from '@ant-design/icons';
import { useNavigate, useLocation, Outlet } from 'react-router-dom';
import './MainLayout.css';

const { Header, Sider, Content, Footer } = Layout;
const { Text } = Typography;

const menuItems = [
  {
    key: '/',
    icon: <HomeOutlined />,
    label: 'Главная',
  },
  {
    key: '/chamber-furnace',
    icon: <FireOutlined />,
    label: 'Камерная печь',
  },
  {
    key: '/drum-dryer',
    icon: <ExperimentOutlined />,
    label: 'Сушильный барабан',
  },
];

export default function MainLayout() {
  const [collapsed, setCollapsed] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();

  const currentKey = '/' + location.pathname.split('/').filter(Boolean).slice(0, 1).join('/');

  const handleMenuClick = ({ key }) => {
    navigate(key);
  };

  return (
    <Layout className="main-layout">
      <Sider
        collapsible
        collapsed={collapsed}
        onCollapse={setCollapsed}
        width={260}
        collapsedWidth={72}
        trigger={null}
        className="main-sider"
        breakpoint="lg"
      >
        {/* Logo */}
        <div className="sider-logo" onClick={() => navigate('/')}>
          <div className="logo-icon">
            <FireOutlined />
          </div>
          {!collapsed && (
            <div className="logo-text">
              <span className="logo-title">Тепловой баланс</span>
              <span className="logo-subtitle">Расчёт параметров</span>
            </div>
          )}
        </div>

        {/* Divider */}
        <div className="sider-divider" />

        {/* Navigation */}
        <Menu
          mode="inline"
          selectedKeys={[currentKey]}
          items={menuItems}
          onClick={handleMenuClick}
          className="sider-menu"
        />

        {/* Bottom info */}
        {!collapsed && (
          <div className="sider-footer">
            <Text className="sider-footer-text">
              УрФУ · 2026
            </Text>
          </div>
        )}
      </Sider>

      <Layout className="main-content-layout">
        {/* Header */}
        <Header className="main-header">
          <Button
            type="text"
            icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
            onClick={() => setCollapsed(!collapsed)}
            className="collapse-btn"
            id="sidebar-toggle"
          />
          <div className="header-right">
            <div className="header-badge">
              <span className="badge-dot" />
              <Text className="badge-text">Система расчётов</Text>
            </div>
          </div>
        </Header>

        {/* Content */}
        <Content className="main-content">
          <Outlet />
        </Content>

        {/* Footer */}
        <Footer className="main-footer">
          <Text type="secondary">
            © {new Date().getFullYear()} Разработка микросервисов расчёта теплотехнических параметров сушильных печей · УрФУ
          </Text>
        </Footer>
      </Layout>
    </Layout>
  );
}
