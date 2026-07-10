// Componente principal da aplicação com roteamento e navegação
import { BrowserRouter, Routes, Route, NavLink, Navigate } from 'react-router-dom';
import PessoasPage from './pages/PessoasPage';
import TransacoesPage from './pages/TransacoesPage';
import TotaisPage from './pages/TotaisPage';

export default function App() {
  return (
    <BrowserRouter>
      <div className="app-layout">
        {/* Barra lateral de navegação */}
        <aside className="sidebar">
          {/* Logo e título do sistema */}
          <div className="sidebar-header">
            <div className="sidebar-logo">💰</div>
            <h1 className="sidebar-title">Controle de Gastos</h1>
            <p className="sidebar-subtitle">Residenciais</p>
          </div>

          {/* Links de navegação */}
          <nav className="sidebar-nav">
            <NavLink
              to="/pessoas"
              className={({ isActive }) => `nav-link ${isActive ? 'nav-link-active' : ''}`}
            >
              <span className="nav-icon">👥</span>
              <span className="nav-text">Pessoas</span>
            </NavLink>
            <NavLink
              to="/transacoes"
              className={({ isActive }) => `nav-link ${isActive ? 'nav-link-active' : ''}`}
            >
              <span className="nav-icon">💸</span>
              <span className="nav-text">Transações</span>
            </NavLink>
            <NavLink
              to="/totais"
              className={({ isActive }) => `nav-link ${isActive ? 'nav-link-active' : ''}`}
            >
              <span className="nav-icon">📈</span>
              <span className="nav-text">Totais</span>
            </NavLink>
          </nav>

          {/* Rodapé da sidebar */}
          <div className="sidebar-footer">
            <p>© 2026 GCES</p>
          </div>
        </aside>

        {/* Conteúdo principal */}
        <main className="main-content">
          <Routes>
            <Route path="/" element={<Navigate to="/pessoas" replace />} />
            <Route path="/pessoas" element={<PessoasPage />} />
            <Route path="/transacoes" element={<TransacoesPage />} />
            <Route path="/totais" element={<TotaisPage />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}
