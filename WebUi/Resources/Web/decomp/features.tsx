import React from 'react';
import { Group } from './types';
import { ToggleSwitch, Slider, ColorPicker, Button, Input } from './ui';

// --- Dynamic List Component (Item Search Logic) ---

const DynamicItemList = ({ items, onChange }: { items: any[], onChange: (items: any[]) => void }) => {
    
    // Default to empty array if undefined
    const list = items || [];

    const handleAdd = () => {
        // Add a new empty item with default color
        const newItem = { id: Date.now(), name: '', color: '#ffffff' };
        onChange([...list, newItem]);
    };

    const handleUpdate = (id: number, field: string, value: any) => {
        const updatedList = list.map(item => {
            if (item.id === id) {
                return { ...item, [field]: value };
            }
            return item;
        });
        onChange(updatedList);
    };

    const handleRemove = (id: number) => {
        onChange(list.filter(i => i.id !== id));
    };

    return (
        <div style={{ display: 'flex', flexDirection: 'column', gap: '5px', marginTop: '5px' }}>
             {/* List Header */}
             {list.length > 0 && (
                <div style={{ display: 'flex', fontSize: '10px', color: '#666', paddingBottom: '2px', borderBottom: '1px solid #222' }}>
                    <div style={{ flex: 1 }}>Name</div>
                    <div style={{ width: '24px', textAlign: 'center' }}>Col</div>
                    <div style={{ width: '24px' }}></div>
                </div>
             )}

            {/* List Items */}
            {list.map((item) => (
                <div key={item.id} style={{ display: 'flex', gap: '5px', alignItems: 'center' }}>
                    <div style={{ flex: 1 }}>
                        <Input 
                            value={item.name} 
                            onChange={(v) => handleUpdate(item.id, 'name', v)} 
                            placeholder="Item name..."
                        />
                    </div>
                    <ColorPicker 
                        color={item.color} 
                        onChange={(c) => handleUpdate(item.id, 'color', c)}
                    />
                    <button 
                        onClick={() => handleRemove(item.id)}
                        style={{ 
                            background: '#2a2f3a', border: '1px solid #333', color: '#ccc', borderRadius: '3px',
                            width: '24px', height: '22px', cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center',
                            fontSize: '12px'
                        }}
                    >
                        <i className="fa-solid fa-trash" style={{ fontSize: '10px', color: '#aaa' }}></i>
                    </button>
                </div>
            ))}

            {/* Add Button */}
            <div style={{ marginTop: '4px' }}>
                 <Button label="Add Item" onClick={handleAdd} />
            </div>
        </div>
    );
};

// --- Group Panel ---

export const GroupPanel = ({ 
    group, featuresState, updateFeature, keybinds, openKeybindModal 
}: { 
    group: Group, featuresState: any, updateFeature: any, keybinds: any, openKeybindModal: any 
}) => (
    <div style={{
        background: 'rgba(20, 22, 27, 0.6)',
        border: '1px solid rgba(255,255,255,0.05)',
        borderRadius: '6px',
        overflow: 'hidden',
        boxShadow: '0 4px 10px rgba(0,0,0,0.2)'
    }}>
        {/* Group Header */}
        <div style={{
            background: 'linear-gradient(90deg, rgba(0, 229, 255, 0.05), transparent)',
            padding: '8px 12px',
            borderBottom: '1px solid rgba(255,255,255,0.03)',
            color: 'var(--accent)',
            fontSize: '11px',
            fontWeight: 700,
            textTransform: 'uppercase',
            letterSpacing: '1px'
        }}>
            {group.label}
        </div>
        {/* Features List */}
        <div style={{ padding: '10px 15px', display: 'flex', flexDirection: 'column', gap: '8px' }}>
            {group.features.map(feature => {
                const val = featuresState[feature.id] ?? feature.defaultValue;
                const bind = keybinds[feature.id];

                return (
                    <div key={feature.id} style={{ display: 'flex', flexDirection: 'column' }}>
                        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', minHeight: '24px' }}>
                            <span style={{ fontSize: '12px', color: '#ccc', fontWeight: 500 }}>{feature.label}</span>
                            <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                                
                                {feature.keybind && (
                                    <div 
                                        onClick={() => openKeybindModal(feature.id, feature.label)}
                                        style={{ 
                                            cursor: 'pointer', padding: '2px', display: 'flex', alignItems: 'center', gap: '4px' 
                                        }}
                                        title={bind ? `Bound to: ${bind}` : 'Set Keybind'}
                                    >
                                        <i className="fa-regular fa-keyboard" style={{ fontSize: '10px', color: bind ? 'var(--accent)' : '#444' }}></i>
                                    </div>
                                )}
                                
                                {feature.type === 'toggle' && (
                                    <ToggleSwitch checked={val} onChange={(v) => updateFeature(feature.id, v)} />
                                )}
                                {feature.type === 'color' && (
                                    <ColorPicker color={val} onChange={(v) => updateFeature(feature.id, v)} />
                                )}
                                {feature.type === 'dropdown' && feature.options && (
                                    <div style={{ position: 'relative' }}>
                                        <select 
                                            value={val} onChange={(e) => updateFeature(feature.id, e.target.value)}
                                            style={{ 
                                                background: '#121418', border: '1px solid #333', color: '#ddd', 
                                                padding: '2px 20px 2px 6px', borderRadius: '3px', fontSize: '11px', 
                                                outline: 'none', cursor: 'pointer', appearance: 'none', 
                                                minWidth: '70px', textAlign: 'left' // Fixed text align and padding
                                            }}
                                        >
                                            {feature.options.map(o => <option key={o} value={o}>{o}</option>)}
                                        </select>
                                        <i className="fa-solid fa-caret-down" style={{ fontSize: '9px', color: '#666', position: 'absolute', right: '6px', top: '5px', pointerEvents: 'none' }}></i>
                                    </div>
                                )}
                            </div>
                        </div>
                        {feature.type === 'slider' && (
                            <div style={{ marginTop: '4px', marginBottom: '4px' }}>
                                <Slider value={val} min={feature.min || 0} max={feature.max || 100} onChange={(v) => updateFeature(feature.id, v)} />
                            </div>
                        )}
                        {feature.type === 'input' && (
                             <div style={{ marginTop: '4px' }}>
                                <Input value={val} onChange={(v) => updateFeature(feature.id, v)} />
                             </div>
                        )}
                        {feature.type === 'button' && (
                             <div style={{ marginTop: '4px' }}>
                                <Button label={feature.label} />
                             </div>
                        )}
                        {feature.type === 'dynamic_list' && (
                             <DynamicItemList items={val} onChange={(v) => updateFeature(feature.id, v)} />
                        )}
                    </div>
                );
            })}
        </div>
    </div>
);