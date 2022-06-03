﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Classes;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace HoraHora
{
    public partial class Top : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string linha = ConfigurationManager.AppSettings["linha"].ToString().Trim();
            //
            string DE = DateTime.Now.Hour.ToString().Length == 1 ? "0" + DateTime.Now.Hour.ToString() + ":00" : DateTime.Now.Hour.ToString() + ":00";
            string ATE = (DateTime.Now.Hour + 1).ToString().Length == 1 ? "0" + (DateTime.Now.Hour + 1).ToString() + ":00" : (DateTime.Now.Hour + 1).ToString() + ":00";
            lbHoraDeAte.Text = DE + " às " + ATE;
            //
            string data = DateTime.Now.Date.ToString("yyyy-MM-dd");//"2019-10-02";
            string horaAtual = DateTime.Now.Hour.ToString().Length == 1 ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString();
            string eventPoint = lbLado.Text;
            //
            Producao producao = new Producao();
            //DESCRIÇÃO DO PARTNUMBER
            List<string> modeloLista = new List<string>();
            modeloLista = producao.Modelo(data, eventPoint, "DESCRICAO");
            //SKUNO DO PARTNUMBER
            List<string> partNumberLista = new List<string>();
            partNumberLista = producao.Modelo(data, eventPoint, "PARTNUMBER");
            string shift = string.Empty;
            //
            Turno turno = new Turno();
            List<Turno.Horario> lista_turno = new List<Turno.Horario>();
            lista_turno = turno.Informacao();
            //
            int rowCount_ = lista_turno.Count;
            if (rowCount_ > 0)
            {
                for (int index = 0; index < rowCount_; index++)
                {
                    string inicioTurno = lista_turno[index].FROMTIME;
                    //
                    if (inicioTurno.Contains(horaAtual))
                    {
                        shift = lista_turno[index].SHIFT;
                        //
                        break;
                    }
                }
            }
            //
            int rowCount = modeloLista.Count;
            //
            if (rowCount > 0)
            {
                if (int.Parse(lbContagem.Text) < rowCount)
                {
                    int row = (int.Parse(lbContagem.Text));
                    //
                    string modelo = partNumberLista[row].ToString();//modeloLista[row].ToString();
                    lbModelo.Text = modeloLista[row].Replace("266-", string.Empty).Replace("001", string.Empty);
                    lbProduto.Text = modeloLista[row].ToString() ;//modeloLista[row].Replace("260-", string.Empty).Replace("000", string.Empty) + " TOP";
                  
                    //
                    List<Producao.Dados> lista = new List<Producao.Dados>();
                    lista = producao.Informacao(data, eventPoint, modelo, shift);
                    //
                    int rowCountLista = lista.Count;
                    if (rowCountLista > 0)
                    {
                        //decimal plan = Convert.ToDecimal(lista[0].PLANEJADO);
                        decimal realizado = 0;
                        decimal falhas = 0;
                        decimal yield = Convert.ToDecimal(lista[0].YIELD.Replace("%", string.Empty));//0;
                        decimal acumPlan = 0;
                        //
                        for (int indice = 0; indice < rowCountLista; indice++)
                        {
                            string horaDe = lista[indice].HOURPERIOD;
                            horaDe = horaDe.Remove(2, 9);
                            acumPlan += Convert.ToDecimal(lista[indice].PLANEJADO);
                            //                       
                            if (horaAtual == horaDe)
                            {
                                realizado += decimal.Parse(lista[indice].REALIZADO);
                                falhas += decimal.Parse(lista[indice].DEFEITO);
                                lbHoraDeAte.Text = lista[indice].HOURPERIOD.Replace("-", " às ");
                                //
                                break;
                            }
                            else
                            {
                                realizado += decimal.Parse(lista[indice].REALIZADO);
                                falhas += decimal.Parse(lista[indice].DEFEITO);
                            }

                        }
                        //
                        lbCirculoPlanejado.Text = acumPlan.ToString();
                        lbCirculoRalizado.Text = realizado.ToString();
                        lbCirculoDefeito.Text = falhas.ToString();
                        //
                        yield = ((realizado - falhas) * 100) / realizado;//acumPlan;
                        double valor = Math.Round(Convert.ToDouble(yield), 2);
                        //yield = ((realizado - falhas) * 100) / acumPlan;
                        //double valor = Convert.ToDouble(yield);//Math.Round(Convert.ToDouble(yield), 2);
                        //double valor = Convert.ToDouble(yield, System.Globalization.CultureInfo.InvariantCulture); // Convert.ToDouble(val_serv, System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                        //valor = Math.Round(valor, 2);
                        //
                        if (valor >= 99.6)
                        {
                            divCirculo.Style.Add("background-color", "#00FF00");
                        }
                        else
                        {
                            divCirculo.Style.Add("background-color", "#FF0000");
                        }
                        //
                        lbPorcentagem.Text = valor.ToString() + "%";
                    }
                    else
                    {
                        lbCirculoPlanejado.Text = "0";
                        lbCirculoRalizado.Text = "0";
                        lbCirculoDefeito.Text = "0";
                        lbPorcentagem.Text = "0%";
                    }
                    //
                    Thread.Sleep(Convert.ToInt32(ConfigurationManager.AppSettings["tempo"].ToString() + "000"));
                }
                //
                if (int.Parse(lbContagem.Text) == Convert.ToInt32(ConfigurationManager.AppSettings["tempo"].ToString()))
                {
                    Response.RedirectToRoute("desempenho_top");
                }
            }
            else
            {
                if (int.Parse(lbContagem.Text) == Convert.ToInt32(ConfigurationManager.AppSettings["tempo"].ToString()))
                {
                    Response.RedirectToRoute("desempenho_top");
                }
            }
            //
            lbContagem.Text = (int.Parse(lbContagem.Text) + 1).ToString();
        }

        //public static double arredonda(double Valor)
        //{
        //    Valor = Valor * 100;
        //    Valor = Math.Round(Valor);
        //    Valor = Valor / 100;
        //    return Valor;
        //}

    }
}