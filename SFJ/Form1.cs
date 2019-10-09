﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SFJ
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        struct Processo
        {
            public int id;
            public int tchegada;
            public int texecucao;
            public int estado; //0 - não iniciado 1 - em espera 2 - execução 3 - terminou
        }

        Random aleatorio = new Random();
        Processo[] vecProcessos = new Processo[200];
        int tSimulacao;
        int tempoSaidaProcessador;
        bool processadorVazio;

        Processo processoNoProcessador;

        //Queue<Processo> filaProcessos = new Queue<Processo>();

        private void start_Click(object sender, EventArgs e)
        {
            tSimulacao = 0;
            timer1.Enabled = true;
            processadorVazio = true;
            for (int i = 0; i < 200; i++)
            {
                vecProcessos[i].id = i;
                vecProcessos[i].tchegada = aleatorio.Next(0, 2000);
                vecProcessos[i].texecucao = aleatorio.Next(10, 50);
                vecProcessos[i].estado = 0;
                list_S.Items.Add(vecProcessos[i].id);
                count1.Text = list_S.Items.Count.ToString();
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            tSimulacao++;
            timer.Text = "" + tSimulacao;

            for (int i = 0; i < 200; i++) //verificar se chegou algum processo
            {
                if (tSimulacao == vecProcessos[i].tchegada) //coloca o processo na fila de espera
                {
                    list_W.Items.Add(vecProcessos[i].id);
                    count2.Text = list_W.Items.Count.ToString();
                    //filaProcessos.Enqueue(vecProcessos[i]);
                    vecProcessos[i].estado = 1;
                }
            }
            int max= list_W.Items.Count;
            for(int i = 0; i < max; i++)
            {
                if (vecProcessos[i].texecucao < 10)
                {
                    vecProcessos[i].id = vecProcessos[0].id;
                }
            }
            if (processadorVazio) //se o processador estiver vazio executa o processo
            {
                if (list_W.Items.Count != 0)
                {
                    
                    //processoNoProcessador = filaProcessos.Dequeue();
                    processoNoProcessador = 0;
                    processadorVazio = false;
                    processoNoProcessador.estado = 2;
                    label_P.Text = processoNoProcessador.id + " ";
                    cputime.Text = processoNoProcessador.texecucao + " ms";
                    tempoSaidaProcessador = tSimulacao + processoNoProcessador.texecucao; //Somar o tempo comutacao
                }
               // if (vecProcessos[0].id ==)
               // {
                   // list_W.Items.Remove(vecProcessos[0].id);
               // }
            }
           
            if (tSimulacao == tempoSaidaProcessador) //verifica se algum processo está a sair do processador
            {
                processadorVazio = true;
                processoNoProcessador.estado = 3;
                list_E.Items.Add(processoNoProcessador.id);
                label_P.Text = "Vazio";
                count3.Text = list_E.Items.Count.ToString();
            }

        }

        private void reset_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void kill_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}