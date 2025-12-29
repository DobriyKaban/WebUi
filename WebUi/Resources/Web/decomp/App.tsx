import React, { useState, useEffect, useRef } from 'react';
import { MENU_DATA } from './data';
// Импортируем компоненты. Убедитесь, что путь './components' верный.
import { GroupPanel, PlayersView, LogsView, KeybindModal } from './components';

const App = () => {
    const [activeTab, setActiveTab] = useState<string>(MENU_DATA[0].id);
    const [activeSubTab, setActiveSubTab] = useState<string>(MENU_DATA[0].subTabs[0].id);
    const [featuresState, setFeaturesState] = useState<Record<string, any>>({});
    const [keybinds, setKeybinds] = useState<Record<string, string>>({});
    const [playersList, setPlayersList] = useState<any[]>([]);
    const [isSidebarHovered, setIsSidebarHovered] = useState(false);
    const [keybindModalInfo, setKeybindModalInfo] = useState<{ id: string, label: string } | null>(null);
    const wsRef = useRef<WebSocket | null>(null);

    // WebSocket Connection
    useEffect(() => {
        const connectWs = () => {
            try {
                const ws = new WebSocket("ws://localhost:4649/");
                ws.onopen = () => console.log("Connected to C# Backend");
                ws.onmessage = (event) => {
                    try {
                        const msg = JSON.parse(event.data);
                        if (msg.type === "player_list") {
                            setPlayersList(msg.data);
                        }
                        // Можно добавить обработку конфига
                    } catch (e) {
                        console.error("WS Message Error", e);
                    }
                };
                ws.onclose = () => setTimeout(connectWs, 2000);
                ws.onerror = (err) => ws.close();
                wsRef.current = ws;
            } catch (e) {
                setTimeout(connectWs, 2000);
            }
        };
        connectWs();
        return () => wsRef.current?.close();
    }, []);

    // Polling for Player List
    useEffect(() => {
        const currentTab = MENU_DATA.find(t => t.id === activeTab);
        const currentSub = currentTab?.subTabs.find(s => s.id === activeSubTab);

        let interval: any = null;

        if (currentSub?.customView === 'players') {
            const fetchPlayers = () => {
                if (wsRef.current?.readyState === WebSocket.OPEN) {
                    wsRef.current.send(JSON.stringify({ type: "get_players" }));
                }
            };
            fetchPlayers(); // Сразу
            interval = setInterval(fetchPlayers, 1000); // И каждую секунду
        }

        return () => {
            if (interval) clearInterval(interval);
        };
    }, [activeTab, activeSubTab]);

    useEffect(() => {
        const tab = MENU_DATA.find(t => t.id === activeTab);
        if (tab && tab.subTabs.length > 0) {
            const isSubTabInNewTab = tab.subTabs.some(sub => sub.id === activeSubTab);
            if (!isSubTabInNewTab) setActiveSubTab(tab.subTabs[0].id);
        }
    }, [activeTab]);

    const currentTab = MENU_DATA.find(t => t.id === activeTab);
    const currentSubTab = currentTab?.subTabs.find(s => s.id === activeSubTab);

    // Feature Update
    const updateFeature = (id: string, value: any) => {
        setFeaturesState(prev => ({ ...prev, [id]: value }));

        if (wsRef.current && wsRef.current.readyState === WebSocket.OPEN) {
            const feature = MENU_DATA.flatMap(t => t.subTabs).flatMap(s => s.groups).flatMap(g => g.features).find(f => f.id === id);

            if (feature?.type === 'button') {
                let payloadValue = value;
                // Для сохранения конфига берем имя из инпута
                if (id === 'cfg_save') {
                    payloadValue = featuresState['cfg_name'] || 'default';
                }

                wsRef.current.send(JSON.stringify({
                    type: 'button_click',
                    id: id,
                    value: payloadValue
                }));
            } else {
                wsRef.current.send(JSON.stringify({
                    type: 'update_feature',
                    id: id,
                    value: value
                }));
            }
        }
    };

    const updateKeybind = (id: string, bind: string) => {
        setKeybinds(prev => ({ ...prev, [id]: bind }));
        if (wsRef.current && wsRef.current.readyState === WebSocket.OPEN) {
            wsRef.current.send(JSON.stringify({
                type: 'update_keybind',
                id: id,
                value: bind
            }));
        }
    };

    const startDrag = () => {
        // @ts-ignore
        if (window.chrome && window.chrome.webview) {
            // @ts-ignore
            window.chrome.webview.postMessage("drag_window");
        }
    };

    const accentColor = featuresState['o_accent'] || '#00e5ff';

    return (
        <div style={{
            width: '100vw', height: '100vh',
            background: 'rgba(15, 16, 20, 0.95)',
            display: 'flex', flexDirection: 'column', overflow: 'hidden',
            color: 'white', fontFamily: "'Rajdhani', sans-serif",
            position: 'relative', '--accent': accentColor
        } as React.CSSProperties}>

            {keybindModalInfo && (
                <KeybindModal
                    featureId={keybindModalInfo.id}
                    label={keybindModalInfo.label}
                    currentBind={keybinds[keybindModalInfo.id] || null}
                    onClose={() => setKeybindModalInfo(null)}
                    onSave={(bind) => updateKeybind(keybindModalInfo.id, bind)}
                />
            )}

            {/* Header */}
            <div onMouseDown={startDrag} style={{
                height: '50px', borderBottom: '1px solid rgba(255,255,255,0.05)',
                display: 'flex', alignItems: 'center', padding: '0 20px',
                justifyContent: 'space-between',
                background: 'linear-gradient(to bottom, rgba(255,255,255,0.03), transparent)',
                zIndex: 10, flexShrink: 0, cursor: 'move', userSelect: 'none'
            }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                    <div style={{ width: '8px', height: '8px', background: 'var(--accent)', borderRadius: '50%', boxShadow: '0 0 10px var(--accent)' }} />
                    <span style={{ fontWeight: 700, fontSize: '18px', letterSpacing: '1px', color: '#fff' }}>ROBUSTER<span style={{ color: 'var(--accent)', fontWeight: 400 }}>HOME</span></span>
                </div>
                <div style={{ fontSize: '11px', color: '#555', fontWeight: 600, letterSpacing: '0.5px' }}>
                    BUILD <span style={{ color: '#fff' }}>3.1.2</span>
                </div>
            </div>

            <div style={{ display: 'flex', flex: 1, overflow: 'hidden', position: 'relative' }}>
                {/* Sidebar */}
                <div
                    onMouseEnter={() => setIsSidebarHovered(true)}
                    onMouseLeave={() => setIsSidebarHovered(false)}
                    style={{
                        width: isSidebarHovered ? '180px' : '60px',
                        background: 'rgba(10, 11, 14, 0.5)', display: 'flex', flexDirection: 'column',
                        paddingTop: '20px', borderRight: '1px solid rgba(255,255,255,0.05)',
                        transition: 'width 0.3s cubic-bezier(0.2, 0.8, 0.2, 1)', zIndex: 20, flexShrink: 0
                    }}
                >
                    {MENU_DATA.map(tab => {
                        const isActive = activeTab === tab.id;
                        return (
                            <div key={tab.id} onClick={() => setActiveTab(tab.id)} style={{
                                height: '40px', marginBottom: '4px', display: 'flex', alignItems: 'center', cursor: 'pointer',
                                color: isActive ? '#fff' : '#666',
                                background: isActive ? 'linear-gradient(90deg, rgba(0, 229, 255, 0.1), transparent)' : 'transparent',
                                borderLeft: isActive ? '3px solid var(--accent)' : '3px solid transparent',
                                transition: 'all 0.2s ease', whiteSpace: 'nowrap', paddingLeft: '18px'
                            }}>
                                <i className={`fa-solid ${tab.icon}`} style={{ width: '24px', fontSize: '16px', color: isActive ? 'var(--accent)' : 'inherit', textAlign: 'center' }}></i>
                                <span style={{
                                    opacity: isSidebarHovered ? 1 : 0, transform: isSidebarHovered ? 'translateX(0)' : 'translateX(10px)',
                                    transition: 'all 0.3s ease', fontWeight: 600, fontSize: '14px', marginLeft: '12px', letterSpacing: '0.5px'
                                }}>
                                    {tab.label.toUpperCase()}
                                </span>
                            </div>
                        );
                    })}
                </div>

                {/* Content */}
                <div style={{ flex: 1, display: 'flex', flexDirection: 'column', background: '#0e1013', overflow: 'hidden' }}>
                    <div style={{
                        height: '40px', display: 'flex', alignItems: 'center', padding: '0 20px', gap: '20px',
                        borderBottom: '1px solid rgba(255,255,255,0.05)', background: 'rgba(0,0,0,0.2)', flexShrink: 0
                    }}>
                        {currentTab?.subTabs.map(sub => {
                            const isActive = activeSubTab === sub.id;
                            return (
                                <div key={sub.id} onClick={() => setActiveSubTab(sub.id)} style={{
                                    fontSize: '13px', fontWeight: 600, color: isActive ? 'white' : '#555',
                                    cursor: 'pointer', padding: '10px 0', position: 'relative', transition: 'color 0.2s',
                                    textTransform: 'uppercase', letterSpacing: '0.5px'
                                }}>
                                    {sub.label}
                                    {isActive && (<div style={{ position: 'absolute', bottom: '0', left: '0', width: '100%', height: '2px', background: 'var(--accent)', boxShadow: '0 0 10px var(--accent)' }} />)}
                                </div>
                            );
                        })}
                    </div>

                    <div style={{ flex: 1, overflowY: 'auto', position: 'relative' }}>
                        {/* Рендеринг контента */}
                        {currentSubTab?.customView === 'players' ?
                            <PlayersView players={playersList} /> :
                            currentSubTab?.customView === 'logs' ? <LogsView /> : (
                                <div className="animate-enter" style={{ padding: '20px', display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '20px', alignItems: 'start' }}>
                                    {/* Левая колонка */}
                                    <div style={{ display: 'flex', flexDirection: 'column', gap: '20px' }}>
                                        {currentSubTab?.groups.filter(g => g.column === 0).map((group) => (
                                            <GroupPanel
                                                key={group.id}
                                                group={group}
                                                featuresState={featuresState}
                                                updateFeature={updateFeature}
                                                keybinds={keybinds}
                                                openKeybindModal={(id, l) => setKeybindModalInfo({ id, label: l })}
                                            />
                                        ))}
                                    </div>
                                    {/* Правая колонка */}
                                    <div style={{ display: 'flex', flexDirection: 'column', gap: '20px' }}>
                                        {currentSubTab?.groups.filter(g => g.column === 1).map((group) => (
                                            <GroupPanel
                                                key={group.id}
                                                group={group}
                                                featuresState={featuresState}
                                                updateFeature={updateFeature}
                                                keybinds={keybinds}
                                                openKeybindModal={(id, l) => setKeybindModalInfo({ id, label: l })}
                                            />
                                        ))}
                                    </div>
                                </div>
                            )
                        }
                    </div>
                </div>
            </div>
        </div>
    );
};
export default App;