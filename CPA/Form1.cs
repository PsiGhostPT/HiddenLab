﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            public int thread;
            public int estado; //0 - não iniciado 1 - em espera 2 - execução 3 - terminou
        }

        Random aleatorio = new Random();
        Processo[] vecProcessos = new Processo[200];
        int tSimulacao;
        int tprocesso;
        int tempoSaidaProcessador;
        bool processadorVazio;
        int buffer;
        int buffer_remove;
        int tBuffer;

        Processo processoNoProcessador;

        Queue<Processo> filaProcessos = new Queue<Processo>();

        private void start_Click(object sender, EventArgs e)
        {
            tSimulacao = 0;
            tprocesso = 0;
            tBuffer = 0;
            buffer = 0;
            buffer_remove = buffer;
            label_buffer.Text = buffer_remove + "";
            debug_log.Text += "Start button Pressed\r\n";
            timer1.Enabled = true;
            timer2.Enabled = true;
            timer3.Enabled = true;
            processadorVazio = true;

            for (int i = 0; i < 200; i++)
            {
                vecProcessos[i].id = i;
                vecProcessos[i].tchegada = aleatorio.Next(10, 2000);
                vecProcessos[i].texecucao = aleatorio.Next(10, 50);
                vecProcessos[i].thread = aleatorio.Next(10, 25);
                vecProcessos[i].estado = 0;
                list_S.Items.Add(vecProcessos[i].id);
                count1.Text = list_S.Items.Count.ToString();
                stop.Enabled = true;
                start.Enabled = false;
            }

        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            tSimulacao++;
            label_timer1.Text = tSimulacao + "";
            if (buffer_remove < 0) // Buffer vazio, bloquear processos
            {
                timer1.Enabled = false;
                timer3.Enabled = true;
                buffer_remove = 0;
                debug_log.Text += "Buffer depleted...Stoping Processes for Regen Buffer. Value: " + buffer_remove + "\r\n";
            }
            if (buffer_remove > 25)//buffer com 25 desbloquear processos
            {
                timer1.Enabled = true;
            }
            if (buffer_remove <= 25) // buffer igual ou inferior a 25 ativar buffer 
            {
                timer3.Enabled = true;
                debug_log.Text += "Buffer depletion is immanent...Executing Failsafe. Value: " + buffer_remove + "\r\n";
            }
            if(buffer_remove == 50 || buffer_remove >50 && buffer_remove < 100) //debug
            {
                debug_log.Text += "Buffer is half-full. Value: " + buffer_remove + "\r\n";
            }
        }
        

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            tprocesso++;
            label_timer2.Text = "" + tprocesso;


            for (int i = 0; i < 200; i++) //verificar se chegou algum processo
            {
                if (tprocesso == vecProcessos[i].tchegada) //coloca o processo na fila de espera
                {
                    list_W.Items.Add(vecProcessos[i].id);
                    count2.Text = list_W.Items.Count.ToString();
                    filaProcessos.Enqueue(vecProcessos[i]);
                    vecProcessos[i].estado = 1;
                }
            }
            if (processadorVazio) //se o processador estiver vazio executa o processo
            {
                if (filaProcessos.Count != 0)
                {
                    processoNoProcessador = filaProcessos.Dequeue();
                    list_W.Items.Remove(processoNoProcessador.id); // Remover o id da lista de espera
                    count2.Text = list_W.Items.Count.ToString();
                    processadorVazio = false;
                    processoNoProcessador.estado = 2;
                    label_P.Text = processoNoProcessador.id + " ";
                    cputime.Text = " " + processoNoProcessador.texecucao + " ";
                    thread.Text = processoNoProcessador.thread + "";
                    tempoSaidaProcessador = tprocesso + processoNoProcessador.texecucao; //Somar o tempo comutacao
                    buffer_remove -= processoNoProcessador.thread;
                }
            }
            if (tprocesso == tempoSaidaProcessador) //verifica se algum processo está a sair do processador
            {
                processadorVazio = true;
                processoNoProcessador.estado = 3;
                list_E.Items.Add(processoNoProcessador.id);
                label_buffer.Text = buffer_remove + "";
                bufferbar.Value = buffer_remove;
                label_P.Text = "Vazio";
                count3.Text = list_E.Items.Count.ToString();
            }
            if(list_E.Items.Count == 200)
            {
                timer1.Enabled = false;
                timer2.Enabled = false;
                timer3.Enabled = false;
                stop.Enabled = false;
                debug_log.Text += "Program Task Finished.\r\n";
            }
        }
        private void timer3_Tick(object sender, EventArgs e)
        {
            tBuffer++;
            label_timer3.Text = "" + tBuffer;
            buffer_remove++;
            label_buffer.Text = buffer_remove + "";
            bufferbar.Value = buffer_remove;
            if (buffer_remove == 100) // buffer cheio desliga o timer
            {
                timer3.Enabled = false;
                debug_log.Text += "Buffer Full\r\n";
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

        private void stop_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                timer1.Enabled = false;
                timer2.Enabled = false;
                timer3.Enabled = false;
                stop.Text = "Unpause";
                debug_log.Text += "Returning...\r\n";
            }
            else
            {
                timer1.Enabled = true;
                timer2.Enabled = true;
                timer3.Enabled = true;
                stop.Text = "Pause";
                debug_log.Text += "Program Paused.\r\n";
            }
        }

        private void list_S_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (list_S.SelectedIndex != -1)
            {
                changeSelectedInfo(list_S.Items[list_S.SelectedIndex].ToString());
            }
        }

        private void changeSelectedInfo(String selectedString)
        {
            int selectedInt = Convert.ToInt32(selectedString);
            Processo selected = vecProcessos[selectedInt];
            label_id.Text = selected.id+"";
            label_exec.Text = selected.texecucao+"";
            label_chegada.Text = selected.tchegada+"";
            label_threads.Text= selected.thread + "";
        }

        private void list_W_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (list_W.SelectedIndex != -1)
            {
                changeSelectedInfo(list_W.Items[list_W.SelectedIndex].ToString());
            }
        }

        private void list_E_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (list_E.SelectedIndex != -1)
            {
                changeSelectedInfo(list_E.Items[list_E.SelectedIndex].ToString());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            debug_log.Text += "Inizializing Program\r\n";
        }
    }
}