import React, { useRef } from 'react';

// --- Base UI Components ---

export const ToggleSwitch = ({ checked, onChange, accent = 'var(--accent)' }: { checked: boolean, onChange: (v: boolean) => void, accent?: string }) => (
  <div 
    onClick={() => onChange(!checked)}
    style={{
      width: '32px',
      height: '16px',
      background: checked ? `rgba(0, 229, 255, 0.2)` : '#1a1d24',
      borderRadius: '20px',
      position: 'relative',
      cursor: 'pointer',
      transition: 'all 0.2s ease',
      border: checked ? `1px solid ${accent}` : '1px solid #3a3f4b',
    }}
  >
    <div style={{
      width: '10px',
      height: '10px',
      background: checked ? accent : '#555',
      borderRadius: '50%',
      position: 'absolute',
      top: '2px', 
      left: checked ? '18px' : '2px',
      transition: 'all 0.2s cubic-bezier(0.2, 0.8, 0.2, 1)', 
      boxShadow: checked ? `0 0 8px ${accent}` : 'none'
    }} />
  </div>
);

export const Slider = ({ value, min, max, onChange }: { value: number, min: number, max: number, onChange: (v: number) => void }) => {
  const percentage = ((value - min) / (max - min)) * 100;
  
  return (
    <div style={{ width: '100%', display: 'flex', alignItems: 'center', gap: '10px' }}>
      <div style={{ flex: 1, position: 'relative', height: '16px', display: 'flex', alignItems: 'center' }}>
        <div style={{ position: 'absolute', width: '100%', height: '3px', background: '#2a303c', borderRadius: '2px' }} />
        <div style={{
          position: 'absolute', width: `${percentage}%`, height: '3px',
          background: 'var(--accent)', borderRadius: '2px', boxShadow: '0 0 8px rgba(0, 229, 255, 0.2)'
        }} />
        <input 
          type="range" min={min} max={max} step={max > 100 ? 1 : 0.01} value={value} 
          onChange={(e) => onChange(Number(e.target.value))}
          style={{ width: '100%', height: '100%', opacity: 0, cursor: 'pointer', position: 'absolute', zIndex: 2, margin: 0 }}
        />
        <div style={{
          position: 'absolute', left: `calc(${percentage}% - 5px)`, width: '10px', height: '10px',
          background: '#fff', borderRadius: '50%', pointerEvents: 'none', zIndex: 1
        }} />
      </div>
      <span style={{ 
         fontSize: '11px', color: '#ccc', minWidth: '30px', textAlign: 'right', fontWeight: 600
      }}>{value.toFixed(max <= 10 ? 2 : 0)}</span>
    </div>
  );
};

export const Button = ({ label, onClick }: { label: string, onClick?: () => void }) => (
  <button onClick={onClick} style={{
    background: 'linear-gradient(180deg, #2a2f3a 0%, #20242c 100%)',
    border: '1px solid #3a3f4b',
    color: '#ccc',
    padding: '4px 0',
    width: '100%',
    borderRadius: '3px',
    cursor: 'pointer',
    fontFamily: 'inherit',
    fontSize: '10px',
    fontWeight: 600,
    textTransform: 'uppercase',
    letterSpacing: '0.5px',
    transition: 'all 0.2s',
  }}
  onMouseOver={(e) => { e.currentTarget.style.borderColor = '#555'; e.currentTarget.style.color = 'white'; }}
  onMouseOut={(e) => { e.currentTarget.style.borderColor = '#3a3f4b'; e.currentTarget.style.color = '#ccc'; }}
  >
    {label}
  </button>
);

export const ColorPicker = ({ color, onChange }: { color: string, onChange?: (c: string) => void }) => {
    const inputRef = useRef<HTMLInputElement>(null);
    return (
        <div 
            onClick={() => inputRef.current?.click()}
            style={{
                width: '24px', height: '14px', borderRadius: '3px',
                background: color, border: '1px solid #444',
                cursor: 'pointer', position: 'relative'
            }}
        >
            <input 
                ref={inputRef}
                type="color" 
                value={color} 
                onChange={(e) => onChange && onChange(e.target.value)}
                style={{ position: 'absolute', opacity: 0, width: '100%', height: '100%', cursor: 'pointer', padding: 0, margin: 0, border: 'none' }} 
            />
        </div>
    );
};

export const Input = ({ value, onChange, placeholder }: { value: string, onChange: (v: string) => void, placeholder?: string }) => (
  <input 
    value={value} onChange={(e) => onChange(e.target.value)}
    placeholder={placeholder}
    style={{
      background: '#121418', border: '1px solid #333', color: 'white',
      padding: '4px 6px', borderRadius: '3px', width: '100%', fontSize: '11px', outline: 'none', fontFamily: 'inherit'
    }}
  />
);
