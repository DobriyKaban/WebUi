import React, { useState } from 'react';
import { PlayerDetailsModal } from './modals';

interface Player {
    name: string;
    charName: string; // Исправлено с char на charName
    entity: string;
    status: string;
    job?: string;
}

export const PlayersView = ({ players }: { players: Player[] }) => {
    const [selectedPlayer, setSelectedPlayer] = useState<Player | null>(null);

    // Используем переданные данные или пустой массив, если players undefined
    const displayPlayers = players || [];

    return (
        <div className="animate-enter" style={{ padding: '0 20px', height: '100%', display: 'flex', flexDirection: 'column', position: 'relative' }}>
            {selectedPlayer && <PlayerDetailsModal player={selectedPlayer} onClose={() => setSelectedPlayer(null)} />}

            <div style={{ display: 'flex', justifyContent: 'space-between', padding: '10px 0', borderBottom: '1px solid rgba(255,255,255,0.05)', color: '#666', fontSize: '11px', fontWeight: 700, textTransform: 'uppercase' }}>
                <div style={{ width: '25%' }}>Name</div>
                <div style={{ width: '25%' }}>Character</div>
                <div style={{ width: '20%' }}>Entity</div>
                <div style={{ width: '15%' }}>Status</div>
                <div style={{ width: '15%', textAlign: 'right' }}>Action</div>
            </div>
            <div style={{ overflowY: 'auto', flex: 1, paddingBottom: '10px' }}>
                {displayPlayers.map((p, i) => (
                    <div key={i} style={{
                        display: 'flex', alignItems: 'center', padding: '8px 0',
                        borderBottom: '1px solid rgba(255,255,255,0.03)', fontSize: '12px'
                    }}>
                        <div style={{ width: '25%', fontWeight: 600, color: '#ddd', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{p.name}</div>
                        <div style={{ width: '25%', color: '#999', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{p.charName}</div>
                        <div style={{ width: '20%', color: '#999' }}>{p.entity}</div>
                        <div style={{ width: '15%', color: p.status === 'Dead' || p.status === 'Offline' ? '#ff4444' : '#00ff88' }}>{p.status}</div>
                        <div style={{ width: '15%', textAlign: 'right' }}>
                            <i
                                className="fa-solid fa-ellipsis"
                                style={{ color: '#666', cursor: 'pointer', padding: '5px' }}
                                onClick={() => setSelectedPlayer(p)}
                            ></i>
                        </div>
                    </div>
                ))}
                {displayPlayers.length === 0 && (
                    <div style={{ padding: '20px', textAlign: 'center', color: '#555', fontSize: '12px' }}>
                        No players found or waiting for data...
                    </div>
                )}
            </div>
        </div>
    );
};

export const LogsView = () => {
    const dummyLogs = [
        { time: '23:58:43', msg: 'System Initialized', color: '#00ff00' },
        { time: '23:59:21', msg: 'Connected to Server', color: '#00ff00' },
        { time: '00:00:12', msg: 'Aimbot Active', color: '#00e5ff' },
    ];

    return (
        <div className="animate-enter" style={{ padding: '15px', height: '100%', fontFamily: "'Roboto Mono', monospace", fontSize: '11px' }}>
            {dummyLogs.map((l, i) => (
                <div key={i} style={{ marginBottom: '4px', borderBottom: '1px solid #1a1d24', paddingBottom: '2px' }}>
                    <span style={{ color: '#666', marginRight: '8px' }}>[{l.time}]</span>
                    <span style={{ color: l.color }}>{l.msg}</span>
                </div>
            ))}
        </div>
    );
};