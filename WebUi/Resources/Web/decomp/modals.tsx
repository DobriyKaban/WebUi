import React, { useState, useEffect } from 'react';

export const KeybindModal = ({ featureId, label, currentBind, onClose, onSave }: { featureId: string, label: string, currentBind: string | null, onClose: () => void, onSave: (bind: string) => void }) => {
    const [bind, setBind] = useState<string>(currentBind || '[None]');
    const [listening, setListening] = useState(false);

    useEffect(() => {
        if (!listening) return;
        const handler = (e: KeyboardEvent) => {
            e.preventDefault();
            e.stopPropagation();
            let key = e.code;
            // Simple mapping
            if (key.startsWith('Key')) key = key.replace('Key', '');
            if (key.startsWith('Digit')) key = key.replace('Digit', '');
            
            // Map special keys for better display
            if (key === 'ArrowUp') key = 'UpArrow';
            if (key === 'ArrowDown') key = 'DownArrow';
            if (key === 'Escape') key = '[None]';

            setBind(key);
            setListening(false);
            onSave(key === '[None]' ? '' : key);
            onClose();
        };
        window.addEventListener('keydown', handler);
        return () => window.removeEventListener('keydown', handler);
    }, [listening, onClose, onSave]);

    // Handle mouse clicks for binding
    const handleMouseDown = (e: React.MouseEvent) => {
        if (!listening) return;
        e.preventDefault();
        e.stopPropagation();
        const key = `Mouse${e.button === 0 ? 'Left' : e.button === 1 ? 'Middle' : 'Right'}`; // Simplified
        // Specialized mapping based on user request (MouseX1/X2 usually buttons 3/4)
        let finalKey = key;
        if(e.button === 3) finalKey = "MouseX1";
        if(e.button === 4) finalKey = "MouseX2";
        
        setBind(finalKey);
        setListening(false);
        onSave(finalKey);
        onClose();
    }

    return (
        <div style={{
            position: 'absolute', top: 0, left: 0, width: '100%', height: '100%',
            background: 'rgba(0,0,0,0.7)', backdropFilter: 'blur(2px)',
            display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000
        }} onClick={onClose}>
            <div style={{
                width: '280px', background: '#111318', border: '1px solid #5865F2', 
                borderRadius: '6px', overflow: 'hidden', boxShadow: '0 10px 30px rgba(0,0,0,0.8)'
            }} onClick={e => e.stopPropagation()}>
                <div style={{
                    background: '#5865F2', padding: '8px 10px', fontWeight: 600, fontSize: '13px', 
                    display: 'flex', justifyContent: 'space-between', alignItems: 'center'
                }}>
                    <span>{label}</span>
                    <i className="fa-solid fa-xmark" style={{ cursor: 'pointer' }} onClick={onClose}></i>
                </div>
                <div style={{ padding: '20px', display: 'flex', flexDirection: 'column', gap: '15px' }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                         <span style={{ fontSize: '14px', color: '#fff' }}>{label}</span>
                         <button 
                            onMouseDown={handleMouseDown}
                            onClick={() => setListening(true)}
                            style={{
                                background: listening ? '#5865F2' : '#2a2f3a',
                                border: '1px solid #444', color: '#fff',
                                padding: '6px 12px', borderRadius: '4px', cursor: 'pointer',
                                minWidth: '80px', fontSize: '12px', fontWeight: 600
                            }}
                         >
                            {listening ? 'Press Key...' : (bind || '[None]')}
                         </button>
                    </div>
                    {/* Delete Bind Button mimicking the screenshot */}
                    <div style={{ display: 'flex', justifyContent: 'flex-end' }}>
                         <button onClick={() => { onSave(''); onClose(); }} style={{
                             background: '#2a2f3a', border: 'none', color: '#fff',
                             padding: '6px 15px', borderRadius: '4px', cursor: 'pointer',
                             fontSize: '12px', fontWeight: 600
                         }}>
                             Delete
                         </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export const PlayerDetailsModal = ({ player, onClose }: { player: any, onClose: () => void }) => (
  <div style={{
    position: 'absolute', top: 0, left: 0, width: '100%', height: '100%',
    background: 'rgba(0,0,0,0.6)', backdropFilter: 'blur(5px)',
    display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 100
  }} onClick={onClose}>
    <div style={{
      width: '320px', background: '#111318', border: '1px solid #333', borderRadius: '4px',
      overflow: 'hidden', boxShadow: '0 20px 50px rgba(0,0,0,0.8)'
    }} onClick={e => e.stopPropagation()}>
      {/* Header */}
      <div style={{
        background: '#5865F2', color: 'white', padding: '6px 10px',
        display: 'flex', justifyContent: 'space-between', alignItems: 'center',
        fontWeight: 600, fontSize: '13px'
      }}>
        <span>Window Title {player.name}</span>
        <i className="fa-solid fa-xmark" style={{ cursor: 'pointer' }} onClick={onClose}></i>
      </div>
      
      {/* Content */}
      <div style={{ display: 'flex', height: '180px' }}>
        <div style={{ width: '120px', display: 'flex', alignItems: 'center', justifyContent: 'center', borderRight: '1px solid #222', color: '#555', fontSize: '12px' }}>
          No Preview
        </div>
        <div style={{ flex: 1, padding: '10px', display: 'flex', flexDirection: 'column', gap: '5px' }}>
          {[
            { label: 'Name', val: player.name || 'Unknown' },
            { label: 'Status', val: player.status || 'Unknown' },
            { label: 'Last Position', val: 'X: 0 Y: 0' },
            { label: 'Job', val: 'Unknown' }
          ].map(row => (
            <div key={row.label} style={{ display: 'flex', flexDirection: 'column', marginBottom: '2px' }}>
               <span style={{ fontSize: '12px', color: '#fff', textAlign: 'center' }}>{row.label}</span>
               <span style={{ fontSize: '11px', color: '#ddd', textAlign: 'center', fontWeight: 600 }}>{row.val}</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  </div>
);
