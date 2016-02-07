//          __  __                                     _   
//         |  \/  |                                   | |  
//         | \  / | _   _  ___   __ _     _ __    ___ | |_ 
//         | |\/| || | | |/ __| / _` |   | '_ \  / _ \| __|
//         | |  | || |_| |\__ \| (_| | _ | | | ||  __/| |_ 
//         |_|  |_| \__,_||___/ \__,_|(_)|_| |_| \___| \__|
//
//  FormulaInterconversionTest.cs
//
//  Author:
//       Davide Guastella <davide.guastella90@gmail.com>
//
//  Copyright (c) 2016 Davide Guastella
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using FormulaLibrary;
using NUnit.Framework;

namespace FormulaLibraryTestProject
{
    /// <summary>
    /// The purpose of this test is to validate formulas by using random generated ones. More precisely, generate random
    /// formulas, then re-parse their string representation using the dedicated parser. In this way, the interconversion
    /// between from formula to string representation, and its reverse (from string to formula) is tested.
    /// </summary>
    [TestFixture]
    class FormulaInterconversionTest
    {
        [TestCase("!(((f(w)&j(g))|(t(r)&m(w))))", ExpectedResult = true)]
        [TestCase("(((l(e)|c(pghy<-float(0.7339609)))&(d(y)|g(v)))|!((d(uvrh<-bool(\"false\"))|g(gjes<-bool(\"false\")))))", ExpectedResult = true)]
        [TestCase("(!(!(x(l)))|((r(g)&r(l))&(l(w)|v(xrov<-float(0.258401)))))", ExpectedResult = true)]
        [TestCase("(!((x(p)&j(suni<-bool(\"false\"))))&((r(o)&d(nlzx<-float(0.3891769)))&!(b(mbcv<-bool(\"true\")))))", ExpectedResult = true)]
        [TestCase("!((!(e(q))|(h(l)|s(t))))", ExpectedResult = true)]
        [TestCase("(!((w(oqer<-float(0.2587696))|q(yhfm<-int(772061452))))&(!(m(h))&(z(i)|s(g))))", ExpectedResult = true)]
        [TestCase("((!(v(ovgb<-bool(\"true\")))|(c(c)|f(m)))|((u(pmgg<-int(44924880))|u(jqqs<-bool(\"true\")))&(o(hyrz<-bool(\"false\"))&m(t))))", ExpectedResult = true)]
        [TestCase("(!((e(u)|p(t)))&!(!(p(lnlz<-string(\"p\")))))", ExpectedResult = true)]
        [TestCase("(!((f(v)|x(mmui<-bool(\"true\"))))|((x(h)&m(s))|(t(w)&h(jylw<-string(\"c\")))))", ExpectedResult = true)]
        [TestCase("(((f(rjpf<-string(\"h\"))&t(e))&(j(p)|n(z)))|!((j(q)|w(ycuz<-int(1541384213)))))", ExpectedResult = true)]
        [TestCase("(!(!(x(g)))&((j(cqyr<-float(0.3891904))&b(r))|(f(kxgy<-bool(\"true\"))&k(s))))", ExpectedResult = true)]
        [TestCase("!(((r(frnw<-int(16347287))&l(cyfp<-string(\"o\")))&!(c(k))))", ExpectedResult = true)]
        [TestCase("!((!(j(tsly<-int(2044629610)))&!(z(k))))", ExpectedResult = true)]
        [TestCase("(!(!(y(j)))|((o(qyjl<-string(\"o\"))|z(fqbg<-bool(\"true\")))|(s(silx<-float(0.914381))&w(dgqu<-int(2131842022)))))", ExpectedResult = true)]
        [TestCase("(((l(bxtt<-int(1543450055))|d(svjb<-int(471448946)))&(o(jjdv<-float(0.02120797))&p(p)))|((f(k)&r(i))|!(x(u))))", ExpectedResult = true)]
        [TestCase("!(((g(cyds<-int(359819581))|w(b))&(r(ywii<-float(0.8668874))|b(r))))", ExpectedResult = true)]
        [TestCase("(((w(i)&m(yzjs<-int(573015940)))&!(w(bhcr<-string(\"q\"))))|((s(vuve<-bool(\"false\"))|u(p))&!(r(f))))", ExpectedResult = true)]
        [TestCase("!(((k(bvki<-bool(\"true\"))&h(p))|!(e(rdxi<-bool(\"true\")))))", ExpectedResult = true)]
        [TestCase("(((u(sdmj<-string(\"i\"))|t(n))|(d(d)|z(r)))|!((m(olpt<-float(0.08419128))&l(g))))", ExpectedResult = true)]
        [TestCase("!(!(!(c(ngzj<-float(0.6549343)))))", ExpectedResult = true)]
        [TestCase("(!((f(g)|w(hxft<-bool(\"false\"))))&!((s(krhw<-bool(\"false\"))|q(j))))", ExpectedResult = true)]
        [TestCase("!((!(j(hjjj<-int(255509278)))&!(u(fomq<-string(\"l\")))))", ExpectedResult = true)]
        [TestCase("(!((i(x)|b(vllk<-int(1709165942))))|((y(xmbk<-int(1678010985))|u(n))&!(f(e))))", ExpectedResult = true)]
        [TestCase("(!((j(c)|h(hgmw<-float(0.7657475))))&!((v(ddyb<-string(\"d\"))|h(c))))", ExpectedResult = true)]
        [TestCase("(((n(ydjb<-bool(\"false\"))&x(k))&(u(xjhd<-float(0.008493698))|p(u)))&((z(ntug<-int(1259905534))&m(ovhw<-bool(\"false\")))|(k(uhew<-int(993096101))&z(rtnt<-bool(\"false\")))))", ExpectedResult = true)]
        [TestCase("(((b(hjdn<-int(1586643601))&s(edib<-float(0.9477809)))&(k(n)&o(b)))&((t(i)&u(bkti<-string(\"y\")))&!(n(e))))", ExpectedResult = true)]
        [TestCase("(!((s(k)&c(o)))&(!(k(flck<-float(0.4916867)))|(y(urvk<-bool(\"false\"))|q(t))))", ExpectedResult = true)]
        [TestCase("(!((w(tohi<-bool(\"true\"))&v(ckre<-float(0.6198906))))|!((g(h)&o(d))))", ExpectedResult = true)]
        [TestCase("(((h(n)&f(c))&!(e(f)))&((d(pbiv<-string(\"t\"))|e(ojnv<-float(0.2058836)))|(q(jhjn<-bool(\"false\"))&h(pztq<-string(\"r\")))))", ExpectedResult = true)]
        [TestCase("((!(y(m))&(q(zhxr<-float(0.4703109))&q(u)))&(!(n(t))|!(t(v))))", ExpectedResult = true)]
        [TestCase("!((!(v(xzno<-bool(\"true\")))|!(s(t))))", ExpectedResult = true)]
        [TestCase("(((j(xlkv<-bool(\"true\"))|q(yftv<-int(310791791)))|(m(z)|g(n)))|!((w(n)|b(vchw<-int(2126892660)))))", ExpectedResult = true)]
        [TestCase("((!(d(v))&(p(vpdz<-string(\"b\"))&g(j)))&!(!(b(w))))", ExpectedResult = true)]
        [TestCase("((!(f(zovb<-int(1058194120)))|!(r(z)))&(!(t(rell<-int(1767966182)))&(b(w)&y(orjn<-float(0.3956785)))))", ExpectedResult = true)]
        [TestCase("!(!((r(p)|w(y))))", ExpectedResult = true)]
        [TestCase("!((!(f(v))|(c(njmt<-bool(\"false\"))&k(u))))", ExpectedResult = true)]
        [TestCase("!(!((k(s)|q(eqjt<-bool(\"false\")))))", ExpectedResult = true)]
        [TestCase("(((w(zree<-float(0.535969))|t(tflg<-bool(\"true\")))|(k(w)&c(k)))&((p(j)&x(h))|!(f(ckly<-int(1119128687)))))", ExpectedResult = true)]
        [TestCase("(((w(rpmi<-string(\"c\"))&o(v))&!(o(e)))|!((o(vlmy<-int(1645074331))&o(j))))", ExpectedResult = true)]
        [TestCase("!((!(c(ihgh<-bool(\"true\")))&(l(c)|c(w))))", ExpectedResult = true)]
        [TestCase("((!(i(pkkn<-bool(\"true\")))|(p(d)&g(gwcs<-bool(\"true\"))))&((j(lomc<-string(\"y\"))|n(gcug<-string(\"v\")))&!(h(i))))", ExpectedResult = true)]
        [TestCase("(((h(f)&c(pcdd<-float(0.1342848)))|(s(buhn<-bool(\"false\"))&j(r)))&!((j(n)&m(z))))", ExpectedResult = true)]
        [TestCase("(((k(bnqj<-bool(\"false\"))|k(uosz<-string(\"z\")))&!(m(s)))|!((g(j)&e(ryye<-float(0.12586)))))", ExpectedResult = true)]
        [TestCase("!((!(u(vtwt<-float(0.4765474)))&(e(ljhl<-string(\"s\"))&k(xycb<-bool(\"false\")))))", ExpectedResult = true)]
        [TestCase("!(!(!(l(e))))", ExpectedResult = true)]
        [TestCase("((!(m(sgww<-string(\"q\")))|!(s(vpmu<-int(1846098846))))|!(!(l(h))))", ExpectedResult = true)]
        [TestCase("(((g(h)&d(rdfo<-int(1497618695)))|!(d(lrru<-bool(\"true\"))))|((w(zojj<-bool(\"false\"))|g(fttd<-int(295043343)))&!(n(htli<-float(0.08148257)))))", ExpectedResult = true)]
        [TestCase("((!(m(wger<-int(1199437743)))|!(g(ynwr<-int(1985035453))))|((t(o)|k(hvwi<-bool(\"false\")))|!(s(ymgr<-int(211364106)))))", ExpectedResult = true)]
        [TestCase("(!(!(u(p)))|!((s(i)&w(p))))", ExpectedResult = true)]
        [TestCase("(((e(ulrf<-string(\"x\"))|g(tquw<-int(471529500)))|!(d(u)))|((i(sbmt<-bool(\"true\"))|f(icfq<-bool(\"false\")))|(r(osfn<-int(802873295))|q(g))))", ExpectedResult = true)]
        [TestCase("(!((d(g)&t(twex<-bool(\"false\"))))&(!(h(t))&(f(rhef<-string(\"r\"))|c(u))))", ExpectedResult = true)]
        [TestCase("!(((y(gkwb<-string(\"c\"))&w(kqqf<-string(\"h\")))|(k(f)&d(b))))", ExpectedResult = true)]
        [TestCase("(!(!(b(yqdq<-int(659731081))))|((h(k)&h(eokv<-bool(\"false\")))&(h(ckbc<-float(0.8166744))&v(n))))", ExpectedResult = true)]
        [TestCase("(((u(w)&b(wrtm<-int(412311556)))|!(g(v)))&((y(mkyj<-int(307710980))&x(d))|(p(tpnk<-string(\"k\"))|q(e))))", ExpectedResult = true)]
        [TestCase("!(((u(m)|o(t))|!(d(pjzy<-float(0.8405433)))))", ExpectedResult = true)]
        [TestCase("!(((w(d)|x(b))|(w(y)&b(urgl<-int(65518168)))))", ExpectedResult = true)]
        [TestCase("((!(f(jkkk<-float(0.4250479)))|!(j(n)))|(!(t(b))&!(j(gfqv<-float(0.5359017)))))", ExpectedResult = true)]
        [TestCase("((!(n(w))&(l(p)&i(n)))|((i(n)&n(fvxv<-bool(\"true\")))&(w(goed<-float(0.5284761))&p(pcce<-float(0.5118134)))))", ExpectedResult = true)]
        [TestCase("(((s(edxh<-string(\"l\"))|j(e))&(x(jqij<-bool(\"true\"))&s(r)))|((q(f)|s(w))|!(e(kxvg<-int(919715178)))))", ExpectedResult = true)]
        [TestCase("!(!((d(m)&y(c))))", ExpectedResult = true)]
        [TestCase("!(((c(b)|g(f))&(f(imys<-bool(\"true\"))|i(t))))", ExpectedResult = true)]
        [TestCase("(((t(n)|w(z))|(o(exuh<-int(345984448))|q(gpib<-bool(\"true\"))))|((k(y)|d(mcpm<-int(1141052670)))&(q(xyki<-string(\"x\"))&h(ojll<-string(\"b\")))))", ExpectedResult = true)]
        [TestCase("!(((k(x)&t(p))|(y(k)&o(v))))", ExpectedResult = true)]
        [TestCase("!((!(t(e))&!(n(i))))", ExpectedResult = true)]
        [TestCase("!((!(m(f))|(q(rbyb<-bool(\"false\"))|z(kjeq<-string(\"e\")))))", ExpectedResult = true)]
        [TestCase("((!(x(u))&!(o(o)))&!((t(d)&c(fqbh<-bool(\"true\")))))", ExpectedResult = true)]
        [TestCase("(!((n(d)&l(hnpp<-int(118916981))))&((c(b)&c(wfkw<-string(\"e\")))&(w(nmrp<-int(106191006))&e(jmen<-string(\"f\")))))", ExpectedResult = true)]
        [TestCase("!(((h(b)|g(teit<-string(\"j\")))&(p(nxgh<-float(0.6292721))|n(x))))", ExpectedResult = true)]
        [TestCase("(((l(v)&l(f))&(r(uvkp<-string(\"r\"))&y(c)))&(!(g(z))|(j(t)&f(s))))", ExpectedResult = true)]
        [TestCase("((!(y(n))|!(e(c)))&!(!(x(d))))", ExpectedResult = true)]
        [TestCase("(!((h(rkom<-float(0.8361952))|s(o)))|(!(r(yvib<-string(\"y\")))|(x(n)&y(olvk<-bool(\"true\")))))", ExpectedResult = true)]
        [TestCase("!((!(y(febo<-int(2051118278)))|!(m(hsdq<-float(0.3920062)))))", ExpectedResult = true)]
        [TestCase("(((f(mhti<-bool(\"false\"))|d(uiys<-bool(\"true\")))&(c(i)|h(i)))|((g(c)|n(ursk<-float(0.2403836)))&(p(y)|l(d))))", ExpectedResult = true)]
        [TestCase("((!(q(fqyy<-bool(\"true\")))|(g(rixk<-int(1497991967))&c(s)))|((b(joyc<-int(131180669))&q(tilx<-float(0.1494943)))&(t(t)&e(bjly<-float(0.8937965)))))", ExpectedResult = true)]
        [TestCase("((!(j(nrgb<-float(0.1601585)))|(v(konk<-int(1854348564))&x(ptlc<-bool(\"true\"))))&((t(c)&o(pxjp<-bool(\"true\")))&!(w(xmng<-string(\"x\")))))", ExpectedResult = true)]
        [TestCase("(((y(uyol<-float(0.8076097))|f(yfiz<-int(1042425176)))&(k(nuju<-float(0.2356515))&f(luqe<-float(0.9744961))))|((q(q)|d(jytk<-float(0.235412)))|!(s(t))))", ExpectedResult = true)]
        [TestCase("(((g(l)&p(p))&!(r(b)))|((u(siis<-bool(\"false\"))|o(dlzh<-float(0.7010975)))|(g(iqfm<-bool(\"true\"))&r(s))))", ExpectedResult = true)]
        [TestCase("(!(!(l(psmt<-bool(\"false\"))))&(!(w(d))|!(f(qqov<-bool(\"true\")))))", ExpectedResult = true)]
        [TestCase("(((h(npic<-int(2085203971))&r(y))|(h(q)&g(wyuk<-bool(\"false\"))))&(!(r(h))|(m(bxny<-float(0.3501547))&n(g))))", ExpectedResult = true)]
        [TestCase("((!(b(povx<-int(2023245870)))|(v(wyyf<-string(\"c\"))|z(hnld<-bool(\"true\"))))|!((n(kqxx<-int(424058276))&s(r))))", ExpectedResult = true)]
        [TestCase("(!((v(s)|l(pfcd<-bool(\"true\"))))|!((n(n)&n(m))))", ExpectedResult = true)]
        [TestCase("(((j(xhmn<-float(0.0954935))&u(h))&(t(u)&z(f)))|((f(drtu<-int(410465500))|n(m))&(l(w)|g(j))))", ExpectedResult = true)]
        [TestCase("(((o(x)&y(o))|(j(gmxp<-string(\"k\"))|k(e)))|(!(y(eqgd<-string(\"v\")))&(n(b)&t(mdbm<-float(0.756211)))))", ExpectedResult = true)]
        [TestCase("(!((k(ulid<-string(\"x\"))&f(x)))&((d(s)|j(l))|!(g(x))))", ExpectedResult = true)]
        [TestCase("(!((x(dylv<-float(0.04564245))&s(kvre<-bool(\"true\"))))&!(!(v(zlwf<-int(1921099901)))))", ExpectedResult = true)]
        [TestCase("(((b(kkyk<-bool(\"false\"))&n(t))|!(v(e)))&((k(trcg<-int(2023767402))&u(m))&!(t(nlig<-string(\"k\")))))", ExpectedResult = true)]
        [TestCase("(!((p(d)&b(d)))&((i(dwdg<-float(0.1547508))&w(r))|(c(nxcr<-string(\"i\"))|c(ipfe<-bool(\"true\")))))", ExpectedResult = true)]
        [TestCase("((!(s(oxtn<-bool(\"false\")))|!(l(l)))&((j(h)&c(w))&(x(iimr<-int(943422929))&j(x))))", ExpectedResult = true)]
        [TestCase("!(((h(c)|b(qlbs<-string(\"k\")))&(m(ydbu<-bool(\"true\"))&s(nhkw<-bool(\"false\")))))", ExpectedResult = true)]
        [TestCase("((!(d(oksb<-float(0.664837)))|!(q(i)))&((s(lkhg<-int(1816247018))&l(o))|(c(r)|n(udze<-int(2055539476)))))", ExpectedResult = true)]
        [TestCase("!((!(c(p))&(r(syjy<-int(1123223136))|f(doro<-string(\"k\")))))", ExpectedResult = true)]
        [TestCase("(((l(k)|k(m))|(u(e)|s(oucb<-string(\"y\"))))|((h(zjcn<-bool(\"false\"))|v(fdrn<-string(\"k\")))|(q(lruq<-float(0.0686042))&h(r))))", ExpectedResult = true)]
        [TestCase("(!(!(h(o)))&(!(g(rknd<-int(1206630969)))&(t(ujol<-bool(\"false\"))&v(i))))", ExpectedResult = true)]
        [TestCase("((!(h(d))&(r(l)|y(wrxg<-float(0.2268101))))&!((m(huby<-bool(\"true\"))|g(voiq<-bool(\"true\")))))", ExpectedResult = true)]
        [TestCase("!(!((o(b)|m(y))))", ExpectedResult = true)]
        [TestCase("((!(v(c))&!(j(o)))&((v(s)|h(r))&(j(o)|f(n))))", ExpectedResult = true)]
        [TestCase("!(((r(x)&j(mqzz<-float(0.1109193)))&(t(otqv<-string(\"q\"))&x(vcvz<-bool(\"false\")))))", ExpectedResult = true)]
        [TestCase("(((q(ltut<-float(0.7200472))&o(c))&!(c(q)))|(!(z(u))|(u(bprj<-int(803954172))|v(vxjy<-bool(\"true\")))))", ExpectedResult = true)]
        [TestCase("(!((u(qrxh<-float(0.2004232))&q(ysup<-int(122275803))))|((o(epsv<-bool(\"true\"))|l(h))&(v(xtcf<-int(390124170))&r(gpjt<-string(\"j\")))))", ExpectedResult = true)]
        [TestCase("(((s(d)&r(y))&(y(rfrm<-float(0.5285633))&v(v)))|((c(ryri<-float(0.4537418))&k(kwgz<-int(1354610743)))|!(j(iseo<-bool(\"false\")))))", ExpectedResult = true)]
        [Test]
        public bool interconversionTest(string formula)
        {
            return new FormulaParser().Parse(formula) != null;
        }
    }
}
