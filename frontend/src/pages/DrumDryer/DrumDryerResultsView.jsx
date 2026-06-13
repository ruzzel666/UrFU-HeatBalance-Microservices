import { Card, Statistic, Row, Col, Segmented, Typography, Collapse, Table, Tag } from 'antd';
import {
  FireOutlined,
  ThunderboltOutlined,
  ExperimentOutlined,
  ArrowLeftOutlined,
  RadiusSettingOutlined,
  DashboardOutlined,
} from '@ant-design/icons';
import { useState } from 'react';
import ResultsTable from '../../components/ResultsTable/ResultsTable';
import HeatBalanceChart from '../../components/HeatBalanceChart/HeatBalanceChart';
import EfficiencyGauge from '../../components/EfficiencyGauge/EfficiencyGauge';
import './DrumDryerPage.css';

const { Title, Text } = Typography;

function fmt(num) {
  if (num == null) return '—';
  return num.toLocaleString('ru-RU', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  });
}

export default function DrumDryerResultsView({ data, onBack }) {
  const [balanceMode, setBalanceMode] = useState('no-recirc');

  const { fuelCombustion, heatBalanceNoRecirculation, heatBalanceWithRecirculation, summary } = data;

  const activeBalance =
    balanceMode === 'no-recirc' ? heatBalanceNoRecirculation : heatBalanceWithRecirculation;

  const aerodynamicsData = Object.values(fuelCombustion.aerodynamics).map((item, i) => ({
    key: i,
    ...item,
  }));

  const oxygenData = Object.values(fuelCombustion.oxygenAndAir).map((item, i) => ({
    key: i,
    ...item,
  }));

  const combustionHeatData = Object.values(fuelCombustion.combustionHeat).map((item, i) => ({
    key: i,
    ...item,
  }));

  const combustionColumns = [
    { title: 'Параметр', dataIndex: 'label', key: 'label' },
    {
      title: 'Значение',
      dataIndex: 'value',
      key: 'value',
      align: 'right',
      render: (val) => <Text strong>{fmt(val)}</Text>,
    },
    {
      title: 'Ед. измерения',
      dataIndex: 'unit',
      key: 'unit',
      width: 120,
      align: 'center',
      render: (unit) => <Tag>{unit || '—'}</Tag>,
    },
    {
      title: 'Обозначение',
      dataIndex: 'symbol',
      key: 'symbol',
      width: 100,
      align: 'center',
      render: (sym) => <Text type="secondary" italic>{sym}</Text>,
    },
  ];

  const productsData = ['co2', 'h2o', 'n2'].map((key) => ({
    key,
    component: fuelCombustion.combustionProductsAlpha1[key]?.label || key,
    alpha1_vol: fuelCombustion.combustionProductsAlpha1[key]?.value,
    alpha1_pct: fuelCombustion.compositionAlpha1[key]?.value,
    alpha115_vol: fuelCombustion.combustionProductsAlpha115[key]?.value,
    alpha115_pct: fuelCombustion.compositionAlpha115[key]?.value,
  }));

  productsData.push({
    key: 'o2excess',
    component: 'O₂ избыточный',
    alpha1_vol: '—',
    alpha1_pct: '—',
    alpha115_vol: fuelCombustion.combustionProductsAlpha115.o2excess?.value,
    alpha115_pct: fuelCombustion.compositionAlpha115.o2excess?.value,
  });

  productsData.push({
    key: 'total',
    component: 'Итого',
    alpha1_vol: fuelCombustion.combustionProductsAlpha1.total?.value,
    alpha1_pct: '100',
    alpha115_vol: fuelCombustion.combustionProductsAlpha115.total?.value,
    alpha115_pct: '100',
  });

  const productsColumns = [
    { title: 'Компонент', dataIndex: 'component', key: 'component', render: (t) => <Text strong={t === 'Итого'}>{t}</Text> },
    {
      title: 'При α = 1',
      children: [
        { title: 'м³/м³', dataIndex: 'alpha1_vol', key: 'a1v', align: 'right', render: (v) => typeof v === 'number' ? fmt(v) : v },
        { title: '%', dataIndex: 'alpha1_pct', key: 'a1p', align: 'right', render: (v) => typeof v === 'number' ? fmt(v) : v },
      ],
    },
    {
      title: 'При α = 1,15',
      children: [
        { title: 'м³/м³', dataIndex: 'alpha115_vol', key: 'a115v', align: 'right', render: (v) => typeof v === 'number' ? fmt(v) : v },
        { title: '%', dataIndex: 'alpha115_pct', key: 'a115p', align: 'right', render: (v) => typeof v === 'number' ? fmt(v) : v },
      ],
    },
  ];

  const collapseItems = [
    {
      key: 'aero',
      label: 'Аэродинамический и температурный режимы',
      children: (
        <Table columns={combustionColumns} dataSource={aerodynamicsData} pagination={false} size="small" bordered />
      ),
    },
    {
      key: 'oxygen',
      label: 'Расчёт кислорода и воздуха',
      children: (
        <Table columns={combustionColumns} dataSource={oxygenData} pagination={false} size="small" bordered />
      ),
    },
    {
      key: 'products',
      label: 'Объём и состав продуктов сгорания',
      children: (
        <Table columns={productsColumns} dataSource={productsData} pagination={false} size="small" bordered />
      ),
    },
    {
      key: 'heat',
      label: 'Теплота сгорания и температуры горения',
      children: (
        <Table columns={combustionColumns} dataSource={combustionHeatData} pagination={false} size="small" bordered />
      ),
    },
  ];

  return (
    <div className="results-view">
      <a className="back-link" onClick={onBack} id="btn-back-to-drum-form">
        <ArrowLeftOutlined /> Назад к форме
      </a>

      {/* Summary Cards — 2 rows for drum dryer (more metrics) */}
      <Row gutter={[16, 16]} className="summary-cards animate-fade-in-up">
        <Col xs={24} sm={12} lg={6}>
          <Card className="summary-card">
            <EfficiencyGauge
              value={summary.efficiency.value}
              label={summary.efficiency.label}
              symbol={summary.efficiency.symbol}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card className="summary-card stat-card">
            <Statistic
              title={summary.fuelConsumption.label}
              value={summary.fuelConsumption.value}
              precision={4}
              suffix={summary.fuelConsumption.unit}
              prefix={<FireOutlined className="stat-icon" />}
            />
            <Text type="secondary" className="stat-symbol">{summary.fuelConsumption.symbol}</Text>
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card className="summary-card stat-card">
            <Statistic
              title={summary.drumVolume.label}
              value={summary.drumVolume.value}
              precision={2}
              suffix={summary.drumVolume.unit}
              prefix={<RadiusSettingOutlined className="stat-icon" />}
            />
            <Text type="secondary" className="stat-symbol">{summary.drumVolume.symbol}</Text>
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card className="summary-card stat-card">
            <Statistic
              title={summary.productivity.label}
              value={summary.productivity.value}
              precision={3}
              suffix={summary.productivity.unit}
              prefix={<DashboardOutlined className="stat-icon" />}
            />
            <Text type="secondary" className="stat-symbol">{summary.productivity.symbol}</Text>
          </Card>
        </Col>
      </Row>

      <Row gutter={[16, 16]} className="summary-cards animate-fade-in-up delay-1" style={{ marginTop: 0 }}>
        <Col xs={24} sm={8}>
          <Card className="summary-card stat-card">
            <Statistic
              title={summary.heatPerKgMoisture.label}
              value={summary.heatPerKgMoisture.value}
              precision={2}
              suffix={summary.heatPerKgMoisture.unit}
              prefix={<ThunderboltOutlined className="stat-icon" />}
            />
            <Text type="secondary" className="stat-symbol">{summary.heatPerKgMoisture.symbol}</Text>
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card className="summary-card stat-card">
            <Statistic
              title={summary.removedMoisture.label}
              value={summary.removedMoisture.value}
              precision={2}
              suffix={summary.removedMoisture.unit}
              prefix={<ExperimentOutlined className="stat-icon" />}
            />
            <Text type="secondary" className="stat-symbol">{summary.removedMoisture.symbol}</Text>
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card className="summary-card stat-card">
            <Statistic
              title={summary.totalFuelHeat.label}
              value={summary.totalFuelHeat.value}
              precision={2}
              suffix={summary.totalFuelHeat.unit}
              prefix={<FireOutlined className="stat-icon" />}
            />
            <Text type="secondary" className="stat-symbol">{summary.totalFuelHeat.symbol}</Text>
          </Card>
        </Col>
      </Row>

      {/* Heat Balance */}
      <Card
        className="results-section animate-fade-in-up delay-2"
        title={
          <div className="section-header">
            <Title level={4} style={{ margin: 0 }}>Тепловой баланс</Title>
            <Segmented
              options={[
                { label: 'Без рециркуляции', value: 'no-recirc' },
                { label: 'С рециркуляцией', value: 'recirc' },
              ]}
              value={balanceMode}
              onChange={setBalanceMode}
              id="drum-balance-mode-toggle"
            />
          </div>
        }
      >
        <Row gutter={[24, 24]}>
          <Col xs={24} xl={14}>
            <ResultsTable
              items={activeBalance.items}
              totalHeat={activeBalance.totalHeat}
            />
          </Col>
          <Col xs={24} xl={10}>
            <HeatBalanceChart
              items={activeBalance.items}
              title="Распределение теплоты"
            />
          </Col>
        </Row>
      </Card>

      {/* Fuel Combustion */}
      <Card
        className="results-section animate-fade-in-up delay-3"
        title={<Title level={4} style={{ margin: 0 }}>Расчёт горения топлива</Title>}
      >
        <Collapse
          items={collapseItems}
          defaultActiveKey={['aero']}
          className="combustion-collapse"
        />
      </Card>
    </div>
  );
}
