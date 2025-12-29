import { MainTab } from './types';

export const MENU_DATA: MainTab[] = [
  {
    id: 'combat',
    label: 'AimBot',
    icon: 'fa-crosshairs',
    subTabs: [
      {
        id: 'gun',
        label: 'Gun',
        icon: 'fa-gun',
        groups: [
          {
            id: 'g_main', label: 'General', column: 0,
            features: [
              { id: 'g_enabled', label: 'Enabled', type: 'toggle', defaultValue: false, keybind: true },
              { id: 'g_radius', label: 'Radius', type: 'slider', min: 0, max: 10, defaultValue: 2.5 },
              { id: 'g_priority', label: 'Priority', type: 'dropdown', options: ['Distance', 'Health', 'Role'], defaultValue: 'Distance' },
              { id: 'g_only_prio', label: 'Only Priority', type: 'toggle', defaultValue: false },
              { id: 'g_crit', label: 'Critical', type: 'toggle', defaultValue: false },
            ]
          },
          {
            id: 'g_aim', label: 'Accuracy', column: 0,
            features: [
              { id: 'g_hitscan', label: 'Hit Scan', type: 'toggle', defaultValue: false },
              { id: 'g_spread', label: 'Minimal Spread', type: 'toggle', defaultValue: false },
            ]
          },
          {
            id: 'g_pred', label: 'Prediction', column: 1,
            features: [
              { id: 'g_autopred', label: 'Auto Predict', type: 'toggle', defaultValue: true },
              { id: 'g_predict', label: 'Predict', type: 'toggle', defaultValue: true },
              { id: 'g_pred_corr', label: 'Correction', type: 'slider', min: 0, max: 2, defaultValue: 0 },
            ]
          },
          {
            id: 'g_auto', label: 'Automation', column: 1,
            features: [
              { id: 'g_ammo', label: 'Show Ammo', type: 'toggle', defaultValue: false },
              { id: 'g_bolt', label: 'Auto Bolt', type: 'toggle', defaultValue: false },
              { id: 'g_reload', label: 'Auto Reload', type: 'toggle', defaultValue: false },
              { id: 'g_reload_delay', label: 'Reload Delay', type: 'slider', min: 0, max: 2, defaultValue: 0.10 },
            ]
          }
        ]
      },
      {
        id: 'melee',
        label: 'Melee',
        icon: 'fa-gavel',
        groups: [
          {
            id: 'm_gen', label: 'General', column: 0,
            features: [
                { id: 'm_heavy_bind', label: 'Heavy Attack Bind', type: 'keybind', defaultValue: 'MouseX1' },
                { id: 'm_enabled', label: 'Enabled', type: 'toggle', defaultValue: false, keybind: true },
              { id: 'm_radius', label: 'Radius', type: 'slider', min: 0, max: 5, defaultValue: 1.5 },
              { id: 'm_priority', label: 'Priority', type: 'dropdown', options: ['Distance', 'Health'], defaultValue: 'Distance' },
              { id: 'm_only_prio', label: 'Only Priority', type: 'toggle', defaultValue: false },
              { id: 'm_crit', label: 'Critical', type: 'toggle', defaultValue: false },
            ]
          },
           {
            id: 'm_logic', label: 'Logic', column: 0,
            features: [
              { id: 'm_predict', label: 'Predict Movement', type: 'toggle', defaultValue: true },
              { id: 'm_raycast', label: 'Wall Check', type: 'toggle', defaultValue: true },
              { id: 'm_rotate', label: 'Rotate To Target', type: 'toggle', defaultValue: true },
              { id: 'm_net', label: 'Fix Network Delay', type: 'toggle', defaultValue: false },
            ]
          },
          {
            id: 'm_vis', label: 'Visuals', column: 1,
            features: [
              { id: 'm_circle', label: 'Draw Radius', type: 'toggle', defaultValue: true },
              { id: 'm_line', label: 'Draw Line', type: 'toggle', defaultValue: true },
              { id: 'm_color', label: 'Color', type: 'color', defaultValue: '#ff0000' },
            ]
          },
          {
            id: 'm_auto', label: 'Automation', column: 1,
            features: [
              { id: 'm_auto_atk', label: 'Auto Attack', type: 'toggle', defaultValue: false },
              { id: 'm_360', label: 'Attack 360', type: 'toggle', defaultValue: false },
              { id: 'm_cooldown', label: 'Ignore Cooldown', type: 'toggle', defaultValue: false },
            ]
          }
        ]
      }
    ]
  },
  {
    id: 'visuals',
    label: 'Visuals',
    icon: 'fa-eye',
    subTabs: [
      {
        id: 'esp',
        label: 'ESP',
        icon: 'fa-border-all',
        groups: [
          {
            id: 'e_elements', label: 'ESP Elements', column: 0,
            features: [
              { id: 'e_enabled', label: 'Enabled', type: 'toggle', defaultValue: false },
              { id: 'e_name', label: 'Name', type: 'toggle', defaultValue: true },
              { id: 'e_ckey', label: 'CKey', type: 'toggle', defaultValue: true },
              { id: 'e_antag', label: 'Antag', type: 'toggle', defaultValue: true },
              { id: 'e_friend', label: 'Friend', type: 'toggle', defaultValue: true },
              { id: 'e_prio', label: 'Priority', type: 'toggle', defaultValue: true },
              { id: 'e_combat', label: 'Combat Mode', type: 'toggle', defaultValue: true },
              { id: 'e_implants', label: 'Implants', type: 'toggle', defaultValue: true },
              { id: 'e_contra', label: 'Contraband', type: 'toggle', defaultValue: true },
              { id: 'e_wep', label: 'Weapon', type: 'toggle', defaultValue: true },
              { id: 'e_slip', label: 'No Slip', type: 'toggle', defaultValue: false },
            ]
          },
          {
            id: 'e_col', label: 'Colors', column: 1,
            features: [
              { id: 'e_c_name', label: 'Name', type: 'color', defaultValue: '#00ffaa' },
              { id: 'e_c_ckey', label: 'CKey', type: 'color', defaultValue: '#ffff00' },
              { id: 'e_c_antag', label: 'Antag', type: 'color', defaultValue: '#ff0000' },
              { id: 'e_c_friend', label: 'Friend', type: 'color', defaultValue: '#00ccff' },
              { id: 'e_c_prio', label: 'Priority', type: 'color', defaultValue: '#ff00ff' },
              { id: 'e_c_combat', label: 'Combat Mode', type: 'color', defaultValue: '#ff0000' },
              { id: 'e_c_impl', label: 'Implants', type: 'color', defaultValue: '#ff9900' },
              { id: 'e_c_cont', label: 'Contraband', type: 'color', defaultValue: '#ff5500' },
              { id: 'e_c_wep', label: 'Weapon', type: 'color', defaultValue: '#ff3300' },
              { id: 'e_c_slip', label: 'No Slip', type: 'color', defaultValue: '#00ffff' },
            ]
          },
          {
            id: 'e_font', label: 'Font Settings', column: 1,
            features: [
              { id: 'e_interval', label: 'Update Interval', type: 'slider', min: 1, max: 60, defaultValue: 15 },
              { id: 'e_font', label: 'Main Font', type: 'dropdown', options: ['Boxfont', 'Arial', 'Mono'], defaultValue: 'Boxfont' },
              { id: 'e_size', label: 'Font Size', type: 'slider', min: 6, max: 24, defaultValue: 10 },
            ]
          }
        ]
      },
      {
        id: 'eye',
        label: 'Eye',
        icon: 'fa-binoculars',
        groups: [
          {
            id: 'y_gen', label: 'General', column: 0,
            features: [
              { id: 'y_fov', label: 'FOV Override', type: 'toggle', defaultValue: false, keybind: true },
              { id: 'y_fullbright', label: 'FullBright', type: 'toggle', defaultValue: false, keybind: true },
              { id: 'y_zoom', label: 'Zoom Level', type: 'slider', min: 1, max: 5, defaultValue: 1, keybind: true },
              { id: 'y_reset', label: 'Reset Zoom', type: 'button' },
            ]
          },
          {
            id: 'y_hud', label: 'HUD', column: 0,
            features: [
              { id: 'y_health', label: 'Health', type: 'toggle', defaultValue: false },
              { id: 'y_stam', label: 'Stamina', type: 'toggle', defaultValue: false },
              { id: 'y_hud_col', label: 'HUD Color', type: 'color', defaultValue: '#ff9999' },
              { id: 'y_antag_i', label: 'Antag Icon', type: 'toggle', defaultValue: true },
              { id: 'y_job_i', label: 'Job Icon', type: 'toggle', defaultValue: true },
              { id: 'y_mind_i', label: 'Mind Shield Icon', type: 'toggle', defaultValue: true },
              { id: 'y_rec_i', label: 'Criminal Record Icon', type: 'toggle', defaultValue: false },
              { id: 'y_syn_i', label: 'Syndicate Icon', type: 'toggle', defaultValue: false },
              { id: 'y_chem', label: 'Chemical Analysis', type: 'toggle', defaultValue: false },
              { id: 'y_shock', label: 'Show Electrocution', type: 'toggle', defaultValue: false },
            ]
          },
          {
            id: 'y_patches', label: 'Patches', column: 1,
            features: [
              { id: 'y_clyde', label: 'No Clyde', type: 'toggle', defaultValue: true },
              { id: 'y_smoke', label: 'No Smoke', type: 'toggle', defaultValue: true },
              { id: 'y_overlay', label: 'No Bad Overlays', type: 'toggle', defaultValue: true },
              { id: 'y_recoil', label: 'No Camera Recoil', type: 'toggle', defaultValue: true },
            ]
          },
          {
             id: 'y_store', label: 'Storage', column: 1,
             features: [
               { id: 'y_st_en', label: 'Show Storage', type: 'toggle', defaultValue: false, keybind: true },
               { id: 'y_st_col', label: 'Highlight Color', type: 'color', defaultValue: '#ff0000' },
             ]
          },
        ]
      },
      {
        id: 'fun',
        label: 'Fun',
        icon: 'fa-face-smile-wink',
        groups: [
            {
                id: 'f_main', label: 'Fun', column: 0,
                features: [
                    { id: 'f_rainbow', label: 'Rainbow UI', type: 'toggle', defaultValue: false },
                    { id: 'f_spin', label: 'Spinbot', type: 'toggle', defaultValue: false },
                ]
            }
        ]
      }
    ]
  },
  {
    id: 'players',
    label: 'Players',
    icon: 'fa-users',
    subTabs: [
      {
        id: 'plist', label: 'Players', icon: 'fa-list', groups: [], customView: 'players'
      },
      {
        id: 'logs', label: 'Logs', icon: 'fa-terminal', groups: [], customView: 'logs'
      }
    ]
  },
  {
    id: 'misc',
    label: 'Misc',
    icon: 'fa-ellipsis',
    subTabs: [
      {
        id: 'misc_gen',
        label: 'Misc',
        icon: 'fa-sliders',
        groups: [
            {
                id: 'mi_game', label: 'Gameplay', column: 0,
                features: [
                  { id: 'mi_soap', label: 'Anti Soap', type: 'toggle', defaultValue: false },
                  { id: 'mi_afk', label: 'Anti AFK', type: 'toggle', defaultValue: false },
                  { id: 'mi_trash', label: 'Trash Talk', type: 'toggle', defaultValue: false },
                ]
            },
            {
                id: 'mi_antiaim', label: 'Anti-Aim', column: 0,
                features: [
                  { id: 'mi_antiaim', label: 'Anti Aim', type: 'toggle', defaultValue: false },
                  { id: 'mi_speed', label: 'Rotation Speed', type: 'slider', min: 100, max: 5000, defaultValue: 2700 },
                ]
            },
            {
                id: 'mi_vis', label: 'Visuals', column: 1,
                features: [
                  { id: 'mi_exp', label: 'Show Explosions', type: 'toggle', defaultValue: false },
                  { id: 'mi_traj', label: 'Show Trajectory', type: 'toggle', defaultValue: false },
                  { id: 'mi_dmg', label: 'Damage Overlay', type: 'toggle', defaultValue: false },
                ]
            }
        ]
      },
      {
        id: 'spammer',
        label: 'Spammer',
        icon: 'fa-bullhorn',
        groups: [
            {
                id: 's_chan', label: 'Channels', column: 0,
                features: [
                    { id: 's_c_prot', label: 'Protect Word', type: 'toggle', defaultValue: false },
                    { id: 's_c_loc', label: 'Local', type: 'toggle', defaultValue: false },
                    { id: 's_c_whis', label: 'Whisper', type: 'toggle', defaultValue: false },
                    { id: 's_c_rad', label: 'Radio', type: 'toggle', defaultValue: true },
                    { id: 's_c_looc', label: 'LOOC', type: 'toggle', defaultValue: false },
                    { id: 's_c_ooc', label: 'OOC', type: 'toggle', defaultValue: false },
                    { id: 's_c_emote', label: 'Emotes', type: 'toggle', defaultValue: false },
                    { id: 's_c_dead', label: 'Dead', type: 'toggle', defaultValue: false },
                    { id: 's_c_admin', label: 'Admin', type: 'toggle', defaultValue: false },
                ]
            },
            {
                id: 's_chat', label: 'Chat Spammer', column: 1,
                features: [
                    { id: 's_ch_en', label: 'Enabled', type: 'toggle', defaultValue: false },
                    { id: 's_ch_del', label: 'Delay (ms)', type: 'slider', min: 50, max: 5000, defaultValue: 200 },
                    { id: 's_ch_txt', label: 'Text', type: 'input', defaultValue: 'https://t.me/RobusterHome' },
                ]
            },
            {
                id: 's_ahelp', label: 'AHelp Spammer', column: 1,
                features: [
                    { id: 's_ah_en', label: 'Enabled', type: 'toggle', defaultValue: false },
                    { id: 's_ah_del', label: 'Delay (ms)', type: 'slider', min: 50, max: 5000, defaultValue: 500 },
                    { id: 's_ah_txt', label: 'Text', type: 'input', defaultValue: 'HELP ME!!!' },
                ]
            }
        ]
      },
      {
        id: 'utility',
        label: 'Utility',
        icon: 'fa-toolbox',
        groups: [
            {
                id: 's_trans', label: 'Translator', column: 0,
                features: [
                    { id: 's_tr_en', label: 'Translate Chat', type: 'toggle', defaultValue: false },
                    { id: 's_tr_lang', label: 'Target Language', type: 'dropdown', options: ['RU', 'EN', 'DE'], defaultValue: 'RU' },
                ]
            },
            {
                id: 's_items', label: 'Item Search', column: 1,
                features: [
                    { id: 's_item_en', label: 'Enabled', type: 'toggle', defaultValue: false },
                    { id: 's_item_name', label: 'Show Name', type: 'toggle', defaultValue: true },
                    // REPLACED Button with Dynamic List feature
                    { id: 's_item_list', label: 'Search Items', type: 'dynamic_list', defaultValue: [] },
                ]
            }
        ]
      }
    ]
  },
  {
    id: 'bots',
    label: 'Bots',
    icon: 'fa-robot',
    subTabs: [
      {
        id: 'bot_gen',
        label: 'General',
        icon: 'fa-microchip',
        groups: [
            {
                id: 'b_main', label: 'Automation', column: 0,
                features: [
                  { id: 'b_path', label: 'Auto Pathing', type: 'toggle', defaultValue: false },
                  { id: 'b_med', label: 'Auto Medic', type: 'toggle', defaultValue: false },
                  { id: 'b_miner', label: 'Auto Miner', type: 'toggle', defaultValue: false },
                ]
            }
        ]
      },
      {
        id: 'bot_other',
        label: 'Other',
        icon: 'fa-ghost',
        groups: [
            {
                id: 'b_follow', label: 'Follow', column: 0,
                features: [
                  { id: 'b_foll_en', label: 'Follow Bot', type: 'toggle', defaultValue: false },
                  { id: 'b_dist', label: 'Distance', type: 'slider', min: 1, max: 20, defaultValue: 3 },
                ]
            }
        ]
      }
    ]
  },
  {
    id: 'exploit',
    label: 'Exploit',
    icon: 'fa-skull',
    subTabs: [
        {
            id: 'exp_main',
            label: 'Main',
            icon: 'fa-bolt',
            groups: [
                {
                    id: 'ex_move', label: 'Movement', column: 0,
                    features: [
                        { id: 'ex_speed', label: 'Speedhack Factor', type: 'slider', min: 1, max: 10, defaultValue: 1 },
                        { id: 'ex_noclip', label: 'Noclip', type: 'toggle', defaultValue: false },
                    ]
                },
                {
                    id: 'ex_int', label: 'Interaction', column: 0,
                    features: [
                        { id: 'ex_unlock', label: 'Unlock All Doors', type: 'button' },
                    ]
                }
            ]
        }
    ]
  },
  {
    id: 'settings',
    label: 'Settings',
    icon: 'fa-gear',
    subTabs: [
      {
        id: 'set_main',
        label: 'Settings',
        icon: 'fa-wrench',
        groups: [
            {
                id: 'st_gen', label: 'General', column: 0,
                features: [
                  { id: 'st_menu', label: 'Show Menu', type: 'toggle', defaultValue: true, keybind: true },
                  { id: 'st_notif', label: 'Notifications', type: 'toggle', defaultValue: false },
                  { id: 'st_ru', label: 'Switch To Russian', type: 'button' },
                  { id: 'st_unload', label: 'Unload Cheat', type: 'button' },
                ]
            },
            {
                id: 'st_theme', label: 'Theme', column: 0,
                features: [
                    { id: 'o_accent', label: 'Accent Color', type: 'color', defaultValue: '#00e5ff' }
                ]
            },
            {
                id: 'st_patches', label: 'Patches', column: 0,
                features: [
                  { id: 'st_admin', label: 'Admin Privilege', type: 'toggle', defaultValue: true },
                  { id: 'st_dmg_friend', label: 'No Damage Friend', type: 'toggle', defaultValue: true },
                  { id: 'st_dmg_fs', label: 'No Damage ForceSay', type: 'toggle', defaultValue: true },
                  { id: 'st_grab', label: 'Anti Screen Grab', type: 'toggle', defaultValue: true },
                ]
            }
        ]
      },
      {
        id: 'configs',
        label: 'Configs',
        icon: 'fa-file-code',
        groups: [
            {
                id: 'cfg_man', label: 'Manager', column: 0,
                features: [
                  { id: 'cfg_name', label: 'Config Name', type: 'input', defaultValue: 'legit_cfg' },
                  { id: 'cfg_save', label: 'Save', type: 'button' },
                  { id: 'cfg_open', label: 'Open Folder', type: 'button' },
                ]
            }
        ]
      }
    ]
  }
];