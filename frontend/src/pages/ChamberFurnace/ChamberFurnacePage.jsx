import { useState } from 'react';
import { calculateChamberFurnace } from '../../api/chamberFurnaceApi';
import ResultsView from './ResultsView';
import {
  Form,
  InputNumber,
  Button,
  Tabs,
  Card,
  Typography,
  Space,
  Tag,
  Divider,
  Table,
  Row,
  Col,
  message,
  Tooltip,
} from 'antd';
import {
  FireOutlined,
  ExperimentOutlined,
  ColumnWidthOutlined,
  ClockCircleOutlined,
  DashboardOutlined,
  SendOutlined,
  InfoCircleOutlined,
  ReloadOutlined,
} from '@ant-design/icons';
import './ChamberFurnacePage.css';

const { Title, Text, Paragraph } = Typography;

/* ===== Default values from Excel ===== */
const defaultValues = {
  // Материал
  dryMaterialMass: 10000,
  initialMoisture: 8,
  finalMoisture: 2,
  initialMaterialTemp: 0,

  // Топливо и воздух
  airFlowCoefficient: 1.15,
  cartAndFurnaceMass: 2000,
  airTemperature: 20,
  recirculateTemp: 230,
  deltaAlpha: 0.3,

  // Геометрия
  brickThickness: 0.12,
  doorThickness: 0.04,
  backfillThickness: 0.08,
  innerDoorArea: 9.25,
  outletDiameter: 0.095,
  furnaceCrossSectionArea: 0.432,
  innerWallArea: 39.5,
  outerWallArea: 46,
  midWallArea: 42.8,
  vaultArea: 28.9,
  stackingHeight: 2,

  // Время
  dryingTime: 28800,
  tempRiseTime: 14400,
  holdingTime: 14400,
  coolingTime: 25200,

  // Температурный режим
  tempSchedule: [
    { period: 0, tStart: 550, tMid: 800, tEnd: 650 },
    { period: 1, tStart: 200, tMid: 400, tEnd: 400 },
    { period: 2, tStart: 100, tMid: 330, tEnd: 350 },
  ],
};

/* ===== Field definitions per tab ===== */
const materialFields = [
  { name: 'dryMaterialMass', label: 'Масса сухого материала', unit: 'кг', symbol: 'G с.м', min: 0.1, max: 1000000 },
  { name: 'initialMoisture', label: 'Начальная влажность форм', unit: '%', symbol: 'W₁нач', min: 0, max: 100 },
  { name: 'finalMoisture', label: 'Конечная влажность форм', unit: '%', symbol: 'W₂кон', min: 0, max: 100 },
  { name: 'initialMaterialTemp', label: 'Начальная температура материала', unit: '°С', symbol: 'tм.нач', min: -50, max: 1000 },
];

const fuelFields = [
  { name: 'airFlowCoefficient', label: 'Коэффициент расхода воздуха', unit: '', symbol: 'α', min: 0.1, max: 10, step: 0.01 },
  { name: 'cartAndFurnaceMass', label: 'Масса выносной топки и тележки', unit: 'кг', symbol: 'Мтоп+тел', min: 0, max: 100000 },
  { name: 'airTemperature', label: 'Температура воздуха', unit: '°С', symbol: 'tвозд', min: -50, max: 100 },
  { name: 'recirculateTemp', label: 'Температура рециркулята в конце сушки', unit: '°С', symbol: 'tрец', min: 0, max: 1000 },
  { name: 'deltaAlpha', label: 'Увеличение коэффициента расхода воздуха (рециркуляция)', unit: '', symbol: 'Δα', min: 0, max: 5, step: 0.01 },
];

const geometryFields = [
  { name: 'brickThickness', label: 'Толщина кирпича', unit: 'м', symbol: 'Sкирп', min: 0.001, max: 10, step: 0.001 },
  { name: 'doorThickness', label: 'Толщина двери', unit: 'м', symbol: 'Sдвери', min: 0.001, max: 10, step: 0.001 },
  { name: 'backfillThickness', label: 'Толщина засыпки', unit: 'м', symbol: 'Sзасып', min: 0.001, max: 10, step: 0.001 },
  { name: 'innerDoorArea', label: 'Площадь внутренней поверхности двери', unit: 'м²', symbol: 'Fдвери', min: 0.01, max: 1000, step: 0.01 },
  { name: 'outletDiameter', label: 'Диаметр отверстия выходного сечения топок', unit: 'м', symbol: 'D', min: 0.001, max: 10, step: 0.001 },
  { name: 'furnaceCrossSectionArea', label: 'Площадь поперечного сечения топки', unit: 'м²', symbol: 'Fсеч', min: 0.001, max: 100, step: 0.001 },
  { name: 'innerWallArea', label: 'Площадь внутренней поверхности стен', unit: 'м²', symbol: 'Fвнутр', min: 0.01, max: 10000, step: 0.01 },
  { name: 'outerWallArea', label: 'Площадь внешней поверхности стен', unit: 'м²', symbol: 'Fвнешн', min: 0.01, max: 10000, step: 0.01 },
  { name: 'midWallArea', label: 'Площадь средней поверхности стен', unit: 'м²', symbol: 'Fсред', min: 0.01, max: 10000, step: 0.01 },
  { name: 'vaultArea', label: 'Площадь свода', unit: 'м²', symbol: 'Fсвода', min: 0.01, max: 10000, step: 0.01 },
  { name: 'stackingHeight', label: 'Высота верха садки над уровнем пола', unit: 'м', symbol: 'L', min: 0.1, max: 50, step: 0.1 },
];

const timeFields = [
  { name: 'dryingTime', label: 'Время сушки', unit: 'с', symbol: 'τсушки', min: 1, max: 1000000, step: 1 },
  { name: 'tempRiseTime', label: 'Время подъёма температуры', unit: 'с', symbol: 'τподъём', min: 1, max: 1000000, step: 1 },
  { name: 'holdingTime', label: 'Время выдержки', unit: 'с', symbol: 'τвыдерж', min: 1, max: 1000000, step: 1 },
  { name: 'coolingTime', label: 'Время охлаждения', unit: 'с', symbol: 'τохл', min: 1, max: 1000000, step: 1 },
];

/* ===== Helper: render fields as a grid ===== */
function renderFieldsGrid(fields) {
  return (
    <Row gutter={[20, 0]}>
      {fields.map((field) => (
        <Col xs={24} md={12} key={field.name}>
          <Form.Item
            name={field.name}
            label={
              <span className="field-label">
                {field.label}
                <Tooltip title={`Обозначение: ${field.symbol}`}>
                  <Text type="secondary" className="field-symbol">
                    ({field.symbol})
                  </Text>
                </Tooltip>
              </span>
            }
            rules={[
              { required: true, message: `Введите значение` },
              {
                type: 'number',
                min: field.min,
                message: `Минимум: ${field.min}`,
              },
              {
                type: 'number',
                max: field.max,
                message: `Максимум: ${field.max}`,
              },
            ]}
          >
            <InputNumber
              style={{ width: '100%' }}
              addonAfter={field.unit || undefined}
              step={field.step || 1}
              precision={field.step && field.step < 1 ? String(field.step).split('.')[1]?.length || 2 : undefined}
              placeholder={`${field.min} — ${field.max}`}
              id={`input-${field.name}`}
            />
          </Form.Item>
        </Col>
      ))}
    </Row>
  );
}

/* ===== Temperature Schedule Table ===== */
function TempScheduleTab({ form }) {
  const columns = [
    {
      title: 'Период',
      dataIndex: 'period',
      key: 'period',
      width: 80,
      render: (val) => <Tag color="volcano">{val}</Tag>,
    },
    {
      title: 'tнач, °С',
      dataIndex: 'tStart',
      key: 'tStart',
      render: (_, record, index) => (
        <Form.Item
          name={['tempSchedule', index, 'tStart']}
          rules={[{ required: true, message: 'Обязательно' }]}
          style={{ margin: 0 }}
        >
          <InputNumber style={{ width: '100%' }} min={0} max={2000} id={`input-tempSchedule-${index}-tStart`} />
        </Form.Item>
      ),
    },
    {
      title: 't, °С',
      dataIndex: 'tMid',
      key: 'tMid',
      render: (_, record, index) => (
        <Form.Item
          name={['tempSchedule', index, 'tMid']}
          rules={[{ required: true, message: 'Обязательно' }]}
          style={{ margin: 0 }}
        >
          <InputNumber style={{ width: '100%' }} min={0} max={2000} id={`input-tempSchedule-${index}-tMid`} />
        </Form.Item>
      ),
    },
    {
      title: 'tкон, °С',
      dataIndex: 'tEnd',
      key: 'tEnd',
      render: (_, record, index) => (
        <Form.Item
          name={['tempSchedule', index, 'tEnd']}
          rules={[{ required: true, message: 'Обязательно' }]}
          style={{ margin: 0 }}
        >
          <InputNumber style={{ width: '100%' }} min={0} max={2000} id={`input-tempSchedule-${index}-tEnd`} />
        </Form.Item>
      ),
    },
  ];

  const data = (form.getFieldValue('tempSchedule') || defaultValues.tempSchedule).map(
    (row, i) => ({ ...row, key: i })
  );

  return (
    <div>
      <Paragraph type="secondary" style={{ marginBottom: 16 }}>
        Изменение температур газовой среды в процессе сушки по периодам (0 — начальный, 1 — подъём, 2 — выдержка).
      </Paragraph>
      <Table
        columns={columns}
        dataSource={data}
        pagination={false}
        bordered
        size="middle"
        className="temp-schedule-table"
      />
    </div>
  );
}

/* ===== Main Page Component ===== */
export default function ChamberFurnacePage() {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [results, setResults] = useState(null);
  const [messageApi, contextHolder] = message.useMessage();

  const handleSubmit = async (values) => {
    setLoading(true);
    try {
      console.log('Submitted values:', values);
      messageApi.success('Данные отправлены на расчёт!');
      const data = await calculateChamberFurnace(values);
      setResults(data);
    } catch {
      messageApi.error('Ошибка при отправке данных');
    } finally {
      setLoading(false);
    }
  };

  const handleReset = () => {
    form.setFieldsValue(defaultValues);
    messageApi.info('Значения сброшены до стандартных');
  };

  const tabItems = [
    {
      key: 'material',
      label: (
        <span>
          <ExperimentOutlined /> Материал
        </span>
      ),
      children: (
        <div className="tab-content">
          <Text type="secondary" className="tab-description">
            Характеристики высушиваемого материала: масса, влажность, начальная температура.
          </Text>
          {renderFieldsGrid(materialFields)}
        </div>
      ),
    },
    {
      key: 'fuel',
      label: (
        <span>
          <FireOutlined /> Топливо и воздух
        </span>
      ),
      children: (
        <div className="tab-content">
          <Text type="secondary" className="tab-description">
            Параметры сжигания топлива: коэффициент расхода воздуха, температуры, рециркуляция.
          </Text>
          {renderFieldsGrid(fuelFields)}
        </div>
      ),
    },
    {
      key: 'geometry',
      label: (
        <span>
          <ColumnWidthOutlined /> Геометрия печи
        </span>
      ),
      children: (
        <div className="tab-content">
          <Text type="secondary" className="tab-description">
            Геометрические размеры камеры: толщины стенок, площади поверхностей, высота садки.
          </Text>
          {renderFieldsGrid(geometryFields)}
        </div>
      ),
    },
    {
      key: 'time',
      label: (
        <span>
          <ClockCircleOutlined /> Время
        </span>
      ),
      children: (
        <div className="tab-content">
          <Text type="secondary" className="tab-description">
            Временные характеристики процесса сушки: время каждого периода в секундах.
          </Text>
          {renderFieldsGrid(timeFields)}
        </div>
      ),
    },
    {
      key: 'tempSchedule',
      label: (
        <span>
          <DashboardOutlined /> Температурный режим
        </span>
      ),
      children: (
        <div className="tab-content">
          <TempScheduleTab form={form} />
        </div>
      ),
    },
  ];

  /* If results are loaded, show ResultsView */
  if (results) {
    return (
      <div className="chamber-furnace-page">
        {contextHolder}

        {/* Page Header */}
        <div className="page-header animate-fade-in-up">
          <div className="page-header-icon">
            <FireOutlined />
          </div>
          <div>
            <Title level={3} style={{ margin: 0 }}>
              Камерная сушильная печь
            </Title>
            <Text type="secondary">Расчёт теплового баланса · Результаты</Text>
          </div>
          <Tag color="volcano" className="page-header-tag">Расчёт выполнен</Tag>
        </div>

        <ResultsView data={results} onBack={() => setResults(null)} />
      </div>
    );
  }

  return (
    <div className="chamber-furnace-page">
      {contextHolder}

      {/* Page Header */}
      <div className="page-header animate-fade-in-up">
        <div className="page-header-icon">
          <FireOutlined />
        </div>
        <div>
          <Title level={3} style={{ margin: 0 }}>
            Камерная сушильная печь
          </Title>
          <Text type="secondary">Расчёт теплового баланса · Ввод исходных данных</Text>
        </div>
        <Tag color="success" className="page-header-tag">25+ параметров</Tag>
      </div>

      {/* Form */}
      <Card className="form-card animate-fade-in-up delay-1">
        <Form
          form={form}
          layout="vertical"
          initialValues={defaultValues}
          onFinish={handleSubmit}
          scrollToFirstError
          requiredMark="optional"
          id="chamber-furnace-form"
        >
          <Tabs
            items={tabItems}
            type="line"
            className="form-tabs"
            size="large"
          />

          <Divider />

          {/* Action Buttons */}
          <div className="form-actions">
            <Space size="middle">
              <Button
                type="primary"
                htmlType="submit"
                loading={loading}
                icon={<SendOutlined />}
                size="large"
                className="submit-btn"
                id="btn-calculate"
              >
                Рассчитать тепловой баланс
              </Button>
              <Button
                onClick={handleReset}
                icon={<ReloadOutlined />}
                size="large"
                id="btn-reset"
              >
                Сбросить
              </Button>
            </Space>
            <Text type="secondary" className="form-hint">
              <InfoCircleOutlined /> Все поля обязательны для заполнения
            </Text>
          </div>
        </Form>
      </Card>
    </div>
  );
}
