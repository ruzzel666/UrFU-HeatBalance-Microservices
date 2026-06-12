import { Typography, Card, Row, Col, Tag, Space } from 'antd';
import {
  FireOutlined,
  ExperimentOutlined,
  ArrowRightOutlined,
  ThunderboltOutlined,
  CalculatorOutlined,
  SafetyCertificateOutlined,
} from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import './HomePage.css';

const { Title, Paragraph, Text } = Typography;

const furnaceTypes = [
  {
    key: 'chamber',
    title: 'Камерная сушильная печь',
    description:
      'Расчёт теплового баланса камерной печи: ввод ~25 технологических параметров, анализ горения топлива, статьи прихода и расхода тепла, КПД.',
    icon: <FireOutlined />,
    path: '/chamber-furnace',
    gradient: 'linear-gradient(135deg, #e8590c 0%, #fa8c16 100%)',
    tag: 'Доступен',
    tagColor: 'success',
    params: '25+ параметров',
  },
  {
    key: 'drum',
    title: 'Сушильный барабан',
    description:
      'Расчёт теплотехнических параметров сушильного барабана: объём, производительность, тепловой баланс процесса сушки.',
    icon: <ExperimentOutlined />,
    path: '/drum-dryer',
    gradient: 'linear-gradient(135deg, #1890ff 0%, #36cfc9 100%)',
    tag: 'В разработке',
    tagColor: 'processing',
    params: 'Скоро',
  },
];

const features = [
  {
    icon: <CalculatorOutlined />,
    title: 'Точный расчёт',
    text: 'Более 30 параметров теплового баланса с точностью до 2 знаков',
  },
  {
    icon: <ThunderboltOutlined />,
    title: 'Мгновенный результат',
    text: 'Результаты расчёта доступны за считанные секунды',
  },
  {
    icon: <SafetyCertificateOutlined />,
    title: 'Валидация данных',
    text: 'Строгая проверка введённых значений перед отправкой',
  },
];

export default function HomePage() {
  const navigate = useNavigate();

  return (
    <div className="home-page">
      {/* Hero Section */}
      <section className="hero-section animate-fade-in-up">
        <div className="hero-glow" />
        <div className="hero-content">
          <Tag color="volcano" className="hero-tag">
            v1.0 · Микросервисная архитектура
          </Tag>
          <Title level={1} className="hero-title">
            Расчёт теплотехнических
            <br />
            <span className="hero-highlight">параметров сушильных печей</span>
          </Title>
          <Paragraph className="hero-description">
            Веб-интерфейс для расчёта теплового баланса различных типов
            промышленных сушильных печей. Вводите исходные данные —
            получайте полный расчёт с визуализацией результатов.
          </Paragraph>
        </div>
      </section>

      {/* Furnace Cards */}
      <section className="furnace-cards-section">
        <Title level={3} className="section-title animate-fade-in-up delay-1">
          Выберите тип печи
        </Title>
        <Row gutter={[24, 24]}>
          {furnaceTypes.map((furnace, index) => (
            <Col xs={24} lg={12} key={furnace.key}>
              <Card
                className={`furnace-card animate-fade-in-up delay-${index + 2}`}
                hoverable
                onClick={() => navigate(furnace.path)}
                id={`card-${furnace.key}`}
              >
                <div className="furnace-card-inner">
                  <div
                    className="furnace-card-icon"
                    style={{ background: furnace.gradient }}
                  >
                    {furnace.icon}
                  </div>
                  <div className="furnace-card-content">
                    <div className="furnace-card-header">
                      <Text strong className="furnace-card-title">
                        {furnace.title}
                      </Text>
                      <Tag color={furnace.tagColor}>{furnace.tag}</Tag>
                    </div>
                    <Paragraph
                      className="furnace-card-desc"
                      type="secondary"
                    >
                      {furnace.description}
                    </Paragraph>
                    <div className="furnace-card-footer">
                      <Tag className="param-tag">{furnace.params}</Tag>
                      <span className="furnace-card-link">
                        Перейти <ArrowRightOutlined />
                      </span>
                    </div>
                  </div>
                </div>
              </Card>
            </Col>
          ))}
        </Row>
      </section>

      {/* Features */}
      <section className="features-section">
        <Row gutter={[24, 24]}>
          {features.map((feature, index) => (
            <Col xs={24} md={8} key={index}>
              <div className={`feature-item animate-fade-in-up delay-${index + 3}`}>
                <div className="feature-icon">{feature.icon}</div>
                <Text strong className="feature-title">
                  {feature.title}
                </Text>
                <Text type="secondary" className="feature-text">
                  {feature.text}
                </Text>
              </div>
            </Col>
          ))}
        </Row>
      </section>
    </div>
  );
}
