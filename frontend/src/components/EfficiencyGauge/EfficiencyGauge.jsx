import { useState, useEffect } from 'react';
import { Progress, Typography } from 'antd';
import './EfficiencyGauge.css';

const { Text } = Typography;

/**
 * Круговой индикатор КПД с цветовой индикацией.
 * Зелёный > 25%, жёлтый 15–25%, красный < 15%.
 * Автоматически уменьшается на мобильных экранах.
 */
export default function EfficiencyGauge({ value, label = 'КПД печи', symbol = 'η' }) {
  const [gaugeSize, setGaugeSize] = useState(160);

  useEffect(() => {
    const updateSize = () => {
      setGaugeSize(window.innerWidth < 480 ? 110 : window.innerWidth < 768 ? 130 : 160);
    };
    updateSize();
    window.addEventListener('resize', updateSize);
    return () => window.removeEventListener('resize', updateSize);
  }, []);

  const getColor = (val) => {
    if (val >= 25) return { stroke: '#52c41a', glow: 'rgba(82, 196, 26, 0.2)' };
    if (val >= 15) return { stroke: '#faad14', glow: 'rgba(250, 173, 20, 0.2)' };
    return { stroke: '#e8590c', glow: 'rgba(232, 89, 12, 0.2)' };
  };

  const color = getColor(value);

  return (
    <div className="efficiency-gauge">
      <div className="gauge-glow" style={{ boxShadow: `0 0 40px ${color.glow}` }} />
      <Progress
        type="circle"
        percent={Math.min(value, 100)}
        size={gaugeSize}
        strokeColor={{
          '0%': color.stroke,
          '100%': color.stroke + 'cc',
        }}
        strokeWidth={8}
        format={() => (
          <div className="gauge-inner">
            <span className="gauge-value">{value.toFixed(2)}</span>
            <span className="gauge-percent">%</span>
          </div>
        )}
      />
      <div className="gauge-label">
        <Text strong>{label}</Text>
        <Text type="secondary" className="gauge-symbol">({symbol})</Text>
      </div>
    </div>
  );
}
