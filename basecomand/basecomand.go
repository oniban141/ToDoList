package basecomand

import (
	"fmt"
	"log"
	"time"
	u "todolist/user"

	"github.com/k0kubun/pp"
)

func PrintToDo(t *u.ToDo_List) {
	log.Print("Печать задачи")
	if t.IsDone {
		fmt.Print("|+| ")
	} else {
		fmt.Print("|-| ")
	}
	fmt.Println(t.Name)
	fmt.Println("-----------------")
	fmt.Println(t.Text)
	fmt.Println("-----------------")
	fmt.Println(t.Date)
	fmt.Println(" ")
}

// command -events
func EventsToDo() {

}

// command -list
func List() {
	pp.Print(u.ToDo)
}

// command -help
func Help() {
	fmt.Println("------------------")
	fmt.Println("add : Добавленение задачи")
	fmt.Println("del : Удаление задачи")
	fmt.Println("done : Отметить задачу как выполненную")
	fmt.Println("list : Вывод всех задач")
	fmt.Println("events : Вывод всех событий")
	fmt.Println("exit : Выйти из программы")
	fmt.Println("-------------------")
}

// command -done
func Done(cmd string) {
	log.Print("Выполнение команды done")
	tasks, ok := u.ToDo[cmd]
	if ok && len(tasks) > 0 {
		tasks[0].IsDone = true
		fmt.Println("Задача отмечена как выполненная!")
		log.Print("Задача ", cmd, " успешно отмечена как выполненная")
	} else {
		fmt.Println("Задача не найдена!")
		log.Print("Задача ", cmd, " не существует")
	}
	date := time.Now().Format("2006-01-02 15:04")
	fmt.Println(date)
}

// command -del
func DeleteTasks(cmd string) {
	delete(u.ToDo, cmd)
	log.Print("Задача ", cmd, " удалена")
}
