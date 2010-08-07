<?php

class CommandListPager extends TablePager
{

	function getQueryInfo()
	{
		return array(
			'tables' => 'hmb_commands',
			'fields' => array('command','description', 'switches', 'accesslevel')
			);
	}

	function getIndexField(){return "command";}
	
	function getRowClass($row){return "commandlistentry-" . $row->accesslevel; }

	function isFieldSortable( $field )
	{ return false; }

	function formatValue( $name, $value )
	{
		if($name == "command")
		{
			$lnk = new Linker();
			return $lnk->link(Title::newFromText($value, NS_COMMAND), $value, false, false, false);
		}
		
		return $name . "." . $value;
	}

	function getDefaultSort()
	{
		return ""; //user_accesslevel";
	}

	function getFieldNames()
	{
		return array(
		
		'command' => "Command",
		'description' => "Description",
		'switches' => "Switches",
		'accesslevel' => "Access Level"
		);
	}

}
