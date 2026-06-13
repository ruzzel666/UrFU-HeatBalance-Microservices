import { useState, useEffect } from 'react';
import { Layout, Menu, Typography, Button, Drawer } from 'antd';
import {
  HomeOutlined,
  FireOutlined,
  ExperimentOutlined,
  MenuFoldOutlined,
  MenuUnfoldOutlined,
  CloseOutlined,
} from '@ant-design/icons';
import { useNavigate, useLocation, Outlet } from 'react-router-dom';
import './MainLayout.css';

const { Header, Sider, Content, Footer } = Layout;
const { Text } = Typography;

const MOBILE_BREAKPOINT = 768;

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
  const [isMobile, setIsMobile] = useState(false);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();

  /* Detect mobile */
  useEffect(() => {
    const checkMobile = () => {
      const mobile = window.innerWidth < MOBILE_BREAKPOINT;
      setIsMobile(mobile);
      if (mobile) {
        setCollapsed(true);
        setDrawerOpen(false);
      }
    };
    checkMobile();
    window.addEventListener('resize', checkMobile);
    return () => window.removeEventListener('resize', checkMobile);
  }, []);

  /* Close drawer on route change */
  useEffect(() => {
    if (isMobile) setDrawerOpen(false);
  }, [location.pathname, isMobile]);

  const currentKey = '/' + location.pathname.split('/').filter(Boolean).slice(0, 1).join('/');

  const handleMenuClick = ({ key }) => {
    navigate(key);
    if (isMobile) setDrawerOpen(false);
  };

  const toggleSidebar = () => {
    if (isMobile) {
      setDrawerOpen(!drawerOpen);
    } else {
      setCollapsed(!collapsed);
    }
  };

  /* Sidebar content — shared between Sider and Drawer */
  const sidebarContent = (
    <>
      {/* Logo */}
      <div className="sider-logo" onClick={() => { navigate('/'); if (isMobile) setDrawerOpen(false); }}>
        <div className="logo-icon">
          <FireOutlined />
        </div>
        <div className="logo-text">
          <span className="logo-title">Тепловой баланс</span>
          <span className="logo-subtitle">Расчёт параметров</span>
        </div>
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
      <div className="sider-footer">
        <Text className="sider-footer-text">
          УрФУ · 2026
        </Text>
      </div>
    </>
  );

  return (
    <Layout className="main-layout">
      {/* Desktop Sider */}
      {!isMobile && (
        <Sider
          collapsible
          collapsed={collapsed}
          onCollapse={setCollapsed}
          width={260}
          collapsedWidth={72}
          trigger={null}
          className="main-sider"
        >
          {/* Logo — collapsed version */}
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
          <div className="sider-divider" />
          <Menu
            mode="inline"
            selectedKeys={[currentKey]}
            items={menuItems}
            onClick={handleMenuClick}
            className="sider-menu"
          />
          {!collapsed && (
            <div className="sider-footer">
              <Text className="sider-footer-text">
                УрФУ · 2026
              </Text>
            </div>
          )}
        </Sider>
      )}

      {/* Mobile Drawer */}
      {isMobile && (
        <Drawer
          placement="left"
          open={drawerOpen}
          onClose={() => setDrawerOpen(false)}
          width={280}
          className="mobile-drawer"
          closable={false}
          styles={{ body: { padding: 0, background: 'linear-gradient(180deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%)' } }}
          extra={
            <Button
              type="text"
              icon={<CloseOutlined />}
              onClick={() => setDrawerOpen(false)}
              className="drawer-close-btn"
            />
          }
        >
          {sidebarContent}
        </Drawer>
      )}

      <Layout className={`main-content-layout ${isMobile ? 'mobile' : ''} ${!isMobile && collapsed ? 'collapsed' : ''} ${!isMobile && !collapsed ? 'expanded' : ''}`}>
        {/* Header */}
        <Header className="main-header">
          <Button
            type="text"
            icon={isMobile ? <MenuUnfoldOutlined /> : (collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />)}
            onClick={toggleSidebar}
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
