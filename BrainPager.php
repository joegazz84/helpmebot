<?php

class BrainPager extends TablePager
{

	function getQueryInfo()
	{
		return array(
			'tables' => 'hmb_brain',
			'fields' => '*'
			);
	}

	function getIndexField(){return "keyword_name";}

	function isFieldSortable( $field )
	{ return true; }

	function formatValue( $name, $value )
	{
		return $value;
	}

	function getDefaultSort()
	{
		return "";
	}

	function getFieldNames()
	{
		return array(
		
			'keyword_name' => "Keyword",
			'keyword_response' => "Response",
			'keyword_action' => "Action?",
		);
	}

}
